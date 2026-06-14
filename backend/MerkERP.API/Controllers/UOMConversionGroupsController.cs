using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.API.Services;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

public record BulkActiveDto(List<int> Ids, bool IsActive);

[ApiController]
[Route("api/[controller]")]
public class UOMConversionGroupsController : ControllerBase
{
	private readonly MerkDbContext _db;
	private readonly ExcelService  _excel;

	public UOMConversionGroupsController(MerkDbContext db, ExcelService excel)
	{
		_db    = db;
		_excel = excel;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await _db.UOMConversionGroup_cs.ToListAsync());

	[HttpGet("nextcode")]
	public async Task<IActionResult> NextCode()
	{
		const string prefix = "UGR-";
		var codes = await _db.UOMConversionGroup_cs
			.Where(g => g.InternalCode != null && g.InternalCode.StartsWith(prefix))
			.Select(g => g.InternalCode!)
			.ToListAsync();

		var maxNum = codes
			.Select(c => int.TryParse(c[prefix.Length..], out var n) ? n : 0)
			.DefaultIfEmpty(0)
			.Max();

		return Ok(new { code = $"{prefix}{(maxNum + 1):D3}" });
	}

	[HttpGet("export-template")]
	public IActionResult ExportTemplate()
	{
		var columns = new (string Label, int Width)[]
		{
			("Internal Code", 18),
			("Name EN",       28),
			("Name AR",       28),
		};

		var bytes = _excel.BuildTemplate(columns);

		return File(bytes,
			"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
			"uom-conversion-groups-template.xlsx");
	}

	[HttpPost("import")]
	public async Task<IActionResult> Import(IFormFile file, [FromForm] long? insertedBy = null)
	{
		if (file is null || file.Length == 0)
			return BadRequest("No file uploaded.");

		var created = 0;
		var errors  = new List<string>();

		var rows = _excel.ReadRows(file.OpenReadStream());

		for (int i = 0; i < rows.Count; i++)
		{
			var row          = rows[i];
			var rowNum       = i + 2;
			var internalCode = row.Length > 0 ? row[0] : "";
			var nameEN       = row.Length > 1 ? row[1] : "";
			var nameAR       = row.Length > 2 ? row[2] : "";

			if (string.IsNullOrWhiteSpace(internalCode) && string.IsNullOrWhiteSpace(nameEN) && string.IsNullOrWhiteSpace(nameAR))
				continue;

			if (string.IsNullOrWhiteSpace(internalCode))
			{ errors.Add($"Row {rowNum}: Internal Code is required."); continue; }

			if (string.IsNullOrWhiteSpace(nameEN))
			{ errors.Add($"Row {rowNum}: Name EN is required."); continue; }

			if (string.IsNullOrWhiteSpace(nameAR))
			{ errors.Add($"Row {rowNum}: Name AR is required."); continue; }

			_db.UOMConversionGroup_cs.Add(new UOMConversionGroup_cs
			{
				InternalCode = internalCode,
				Name_EN      = nameEN,
				Name_AR      = nameAR,
				IsActive     = true,
				IsFavorite   = false,
				InsertedBy   = insertedBy,
				InsertedDate = DateTime.UtcNow,
			});
			created++;
		}

		if (created > 0) await _db.SaveChangesAsync();

		return Ok(new { created, errors });
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> Get(int id) =>
		await _db.UOMConversionGroup_cs.FindAsync(id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(UOMConversionGroup_cs e)
	{
		_db.UOMConversionGroup_cs.Add(e);
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Update(int id, UOMConversionGroup_cs e)
	{
		if (id != e.Id) return BadRequest();
		_db.UOMConversionGroup_cs.Update(e);
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
	{
		var e = await _db.UOMConversionGroup_cs.FindAsync(id);
		if (e is null) return NotFound();

		var usageCount = await _db.UOMConversionFactor_cs
			.CountAsync(f => f.UOMConversionGroupId == id);
		if (usageCount > 0)
			return Conflict(new { message = $"Cannot delete: this group is used in {usageCount} conversion factor(s)." });

		_db.UOMConversionGroup_cs.Remove(e);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id:int}/toggle-favorite")]
	public async Task<IActionResult> ToggleFavorite(int id)
	{
		var e = await _db.UOMConversionGroup_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsFavorite = !e.IsFavorite;
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpGet("{id:int}/factors-count")]
	public async Task<IActionResult> FactorsCount(int id) =>
		Ok(new { count = await _db.UOMConversionFactor_cs.CountAsync(f => f.UOMConversionGroupId == id) });

	[HttpPatch("{id:int}/toggle-active")]
	public async Task<IActionResult> ToggleActive(int id)
	{
		var e = await _db.UOMConversionGroup_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsActive = !e.IsActive;

		var factors = await _db.UOMConversionFactor_cs
			.Where(f => f.UOMConversionGroupId == id)
			.ToListAsync();
		factors.ForEach(f => f.IsActive = e.IsActive);

		await _db.SaveChangesAsync();
		return Ok(new { group = e, affectedFactorsCount = factors.Count });
	}

	[HttpPatch("bulk-active")]
	public async Task<IActionResult> BulkSetActive([FromBody] BulkActiveDto dto)
	{
		var entities = await _db.UOMConversionGroup_cs
			.Where(g => dto.Ids.Contains(g.Id))
			.ToListAsync();
		entities.ForEach(e => e.IsActive = dto.IsActive);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] List<int> ids)
	{
		var usedCount = await _db.UOMConversionFactor_cs
			.Where(f => f.UOMConversionGroupId.HasValue && ids.Contains(f.UOMConversionGroupId.Value))
			.Select(f => f.UOMConversionGroupId!.Value)
			.Distinct()
			.CountAsync();
		if (usedCount > 0)
			return Conflict(new { message = $"Cannot delete: {usedCount} of the selected group(s) are used in conversion factors." });

		var entities = await _db.UOMConversionGroup_cs
			.Where(g => ids.Contains(g.Id))
			.ToListAsync();
		_db.UOMConversionGroup_cs.RemoveRange(entities);
		await _db.SaveChangesAsync();
		return NoContent();
	}
}
