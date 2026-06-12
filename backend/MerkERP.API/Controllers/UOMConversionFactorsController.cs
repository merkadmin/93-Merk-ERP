using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.API.Services;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

public record BulkFactorActiveDto(List<long> Ids, bool IsActive);

[ApiController]
[Route("api/[controller]")]
public class UOMConversionFactorsController : ControllerBase
{
	private readonly MerkDbContext _db;
	private readonly ExcelService  _excel;

	public UOMConversionFactorsController(MerkDbContext db, ExcelService excel)
	{
		_db    = db;
		_excel = excel;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await _db.UOMConversionFactor_cs
			.Include(f => f.UOMFrom)
			.Include(f => f.UOMTo)
			.Include(f => f.UOMConversionGroup)
			.OrderBy(f => f.UOMFrom.Name_EN)
			.ToListAsync());

	[HttpGet("nextcode")]
	public async Task<IActionResult> NextCode()
	{
		const string prefix = "UCF-";
		var codes = await _db.UOMConversionFactor_cs
			.Where(f => f.InternalCode != null && f.InternalCode.StartsWith(prefix))
			.Select(f => f.InternalCode!)
			.ToListAsync();

		var maxNum = codes
			.Select(c => int.TryParse(c[prefix.Length..], out var n) ? n : 0)
			.DefaultIfEmpty(0)
			.Max();

		return Ok(new { code = $"{prefix}{(maxNum + 1):D3}" });
	}

	[HttpGet("export-template")]
	public async Task<IActionResult> ExportTemplate()
	{
		var groups = await _db.UOMConversionGroup_cs.OrderBy(g => g.Name_EN).ToListAsync();
		var uoms   = await _db.UOM_cs.OrderBy(u => u.Name_EN).ToListAsync();

		var columns = new (string Label, int Width)[]
		{
			("Internal Code",                18),
			("Conversion Group (Name EN)",   32),
			("From UOM (Name EN)",           28),
			("To UOM (Name EN)",             28),
			("Factor",                       12),
		};

		var referenceSheets = new ReferenceSheet[]
		{
			new("ConversionGroups",
				["Internal Code", "Name EN", "Name AR"],
				groups.Select(g => new[] { g.InternalCode ?? "", g.Name_EN, g.Name_AR ?? "" })),

			new("UOMs",
				["Internal Code", "Name EN", "Name AR"],
				uoms.Select(u => new[] { u.InternalCode ?? "", u.Name_EN, u.Name_AR ?? "" })),
		};

		var bytes = _excel.BuildTemplate(columns, referenceSheets);

		return File(bytes,
			"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
			"uom-conversion-factors-template.xlsx");
	}

	[HttpPost("import")]
	public async Task<IActionResult> Import(IFormFile file)
	{
		if (file is null || file.Length == 0)
			return BadRequest("No file uploaded.");

		var groups = await _db.UOMConversionGroup_cs.ToListAsync();
		var uoms   = await _db.UOM_cs.ToListAsync();

		var created = 0;
		var errors  = new List<string>();

		var rows = _excel.ReadRows(file.OpenReadStream());

		for (int i = 0; i < rows.Count; i++)
		{
			var row          = rows[i];
			var rowNum       = i + 2;
			var internalCode = row.Length > 0 ? row[0] : "";
			var groupName    = row.Length > 1 ? row[1] : "";
			var fromName     = row.Length > 2 ? row[2] : "";
			var toName       = row.Length > 3 ? row[3] : "";
			var factorStr    = row.Length > 4 ? row[4] : "";

			if (string.IsNullOrWhiteSpace(groupName) && string.IsNullOrWhiteSpace(fromName) && string.IsNullOrWhiteSpace(toName))
				continue;

			var group = groups.FirstOrDefault(g =>
				string.Equals(g.Name_EN,      groupName, StringComparison.OrdinalIgnoreCase) ||
				string.Equals(g.Name_AR,      groupName, StringComparison.OrdinalIgnoreCase) ||
				string.Equals(g.InternalCode, groupName, StringComparison.OrdinalIgnoreCase));

			if (group is null) { errors.Add($"Row {rowNum}: Conversion Group \"{groupName}\" not found."); continue; }

			var fromUom = uoms.FirstOrDefault(u =>
				string.Equals(u.Name_EN,      fromName, StringComparison.OrdinalIgnoreCase) ||
				string.Equals(u.Name_AR,      fromName, StringComparison.OrdinalIgnoreCase) ||
				string.Equals(u.InternalCode, fromName, StringComparison.OrdinalIgnoreCase));

			if (fromUom is null) { errors.Add($"Row {rowNum}: From UOM \"{fromName}\" not found."); continue; }

			var toUom = uoms.FirstOrDefault(u =>
				string.Equals(u.Name_EN,      toName, StringComparison.OrdinalIgnoreCase) ||
				string.Equals(u.Name_AR,      toName, StringComparison.OrdinalIgnoreCase) ||
				string.Equals(u.InternalCode, toName, StringComparison.OrdinalIgnoreCase));

			if (toUom is null) { errors.Add($"Row {rowNum}: To UOM \"{toName}\" not found."); continue; }

			if (!double.TryParse(factorStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var factor) || factor <= 0)
			{ errors.Add($"Row {rowNum}: Invalid factor \"{factorStr}\". Must be a positive number."); continue; }

			_db.UOMConversionFactor_cs.Add(new UOMConversionFactor_cs
			{
				InternalCode         = string.IsNullOrWhiteSpace(internalCode) ? null : internalCode,
				UOMConversionGroupId = group.Id,
				UOMFromId            = fromUom.Id,
				UOMToId              = toUom.Id,
				Value                = factor,
				IsActive             = true,
				InsertedDate         = DateTime.UtcNow,
			});
			created++;
		}

		if (created > 0) await _db.SaveChangesAsync();

		return Ok(new { created, errors });
	}

	[HttpGet("{id:long}")]
	public async Task<IActionResult> Get(long id) =>
		await _db.UOMConversionFactor_cs
			.Include(f => f.UOMFrom)
			.Include(f => f.UOMTo)
			.Include(f => f.UOMConversionGroup)
			.FirstOrDefaultAsync(f => f.Id == id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(UOMConversionFactor_cs e)
	{
		e.UOMFrom            = null;
		e.UOMTo              = null;
		e.UOMConversionGroup = null;
		e.InsertedDate       = DateTime.UtcNow;
		_db.UOMConversionFactor_cs.Add(e);
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id:long}")]
	public async Task<IActionResult> Update(long id, UOMConversionFactor_cs e)
	{
		if (id != e.Id) return BadRequest();
		var existing = await _db.UOMConversionFactor_cs.FindAsync(id);
		if (existing is null) return NotFound();
		existing.UOMFromId             = e.UOMFromId;
		existing.UOMToId               = e.UOMToId;
		existing.Value                 = e.Value;
		existing.UOMConversionGroupId  = e.UOMConversionGroupId;
		existing.InternalCode          = e.InternalCode;
		existing.IsActive              = e.IsActive;
		existing.IsFavorite            = e.IsFavorite;
		await _db.SaveChangesAsync();
		return Ok(existing);
	}

	[HttpDelete("{id:long}")]
	public async Task<IActionResult> Delete(long id)
	{
		var e = await _db.UOMConversionFactor_cs.FindAsync(id);
		if (e is null) return NotFound();
		_db.UOMConversionFactor_cs.Remove(e);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id:long}/toggle-favorite")]
	public async Task<IActionResult> ToggleFavorite(long id)
	{
		var e = await _db.UOMConversionFactor_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsFavorite = !e.IsFavorite;
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("{id:long}/toggle-active")]
	public async Task<IActionResult> ToggleActive(long id)
	{
		var e = await _db.UOMConversionFactor_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsActive = !e.IsActive;
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("bulk-active")]
	public async Task<IActionResult> BulkSetActive([FromBody] BulkFactorActiveDto dto)
	{
		var entities = await _db.UOMConversionFactor_cs
			.Where(f => dto.Ids.Contains(f.Id))
			.ToListAsync();
		entities.ForEach(e => e.IsActive = dto.IsActive);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] List<long> ids)
	{
		var entities = await _db.UOMConversionFactor_cs
			.Where(f => ids.Contains(f.Id))
			.ToListAsync();
		_db.UOMConversionFactor_cs.RemoveRange(entities);
		await _db.SaveChangesAsync();
		return NoContent();
	}
}
