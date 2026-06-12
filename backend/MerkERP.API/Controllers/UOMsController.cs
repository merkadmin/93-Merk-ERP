using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.API.Services;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

public record BulkUomActiveDto(List<long> Ids, bool IsActive);

[ApiController]
[Route("api/[controller]")]
public class UOMsController : ControllerBase
{
	private readonly MerkDbContext _db;
	private readonly ExcelService  _excel;

	public UOMsController(MerkDbContext db, ExcelService excel)
	{
		_db    = db;
		_excel = excel;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await _db.UOM_cs.ToListAsync());

	[HttpGet("nextcode")]
	public async Task<IActionResult> NextCode()
	{
		const string prefix = "UOM-";
		var codes = await _db.UOM_cs
			.Where(u => u.InternalCode != null && u.InternalCode.StartsWith(prefix))
			.Select(u => u.InternalCode!)
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
			("Internal Code",    18),
			("Name EN",          28),
			("Name AR",          28),
			("Whole Number Only (yes/no)", 26),
		};

		return File(_excel.BuildTemplate(columns),
			"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
			"uoms-template.xlsx");
	}

	[HttpPost("import")]
	public async Task<IActionResult> Import(IFormFile file)
	{
		if (file is null || file.Length == 0)
			return BadRequest("No file uploaded.");

		var created = 0;
		var errors  = new List<string>();
		var rows    = _excel.ReadRows(file.OpenReadStream());

		for (int i = 0; i < rows.Count; i++)
		{
			var row          = rows[i];
			var rowNum       = i + 2;
			var internalCode = row.Length > 0 ? row[0] : "";
			var nameEN       = row.Length > 1 ? row[1] : "";
			var nameAR       = row.Length > 2 ? row[2] : "";
			var wholeNumStr  = row.Length > 3 ? row[3] : "";

			if (string.IsNullOrWhiteSpace(internalCode) && string.IsNullOrWhiteSpace(nameEN))
				continue;

			if (string.IsNullOrWhiteSpace(internalCode))
			{ errors.Add($"Row {rowNum}: Internal Code is required."); continue; }

			if (string.IsNullOrWhiteSpace(nameEN))
			{ errors.Add($"Row {rowNum}: Name EN is required."); continue; }

			var mustBeWholeNumber = wholeNumStr.Trim().ToLowerInvariant() is "yes" or "true" or "1";

			_db.UOM_cs.Add(new UOM_cs
			{
				InternalCode      = internalCode,
				Name_EN           = nameEN,
				Name_AR           = string.IsNullOrWhiteSpace(nameAR) ? null : nameAR,
				MustBeWholeNumber = mustBeWholeNumber,
				IsActive          = true,
				InsertedDate      = DateTime.UtcNow,
			});
			created++;
		}

		if (created > 0) await _db.SaveChangesAsync();
		return Ok(new { created, errors });
	}

	[HttpGet("{id:long}")]
	public async Task<IActionResult> Get(long id) =>
		await _db.UOM_cs.FindAsync(id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(UOM_cs e)
	{
		e.DefaultForItems = [];
		e.Conversions     = [];
		_db.UOM_cs.Add(e);
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id:long}")]
	public async Task<IActionResult> Update(long id, UOM_cs e)
	{
		if (id != e.Id) return BadRequest();
		var existing = await _db.UOM_cs.FindAsync(id);
		if (existing is null) return NotFound();
		existing.InternalCode      = e.InternalCode;
		existing.Name_EN           = e.Name_EN;
		existing.Name_AR           = e.Name_AR;
		existing.MustBeWholeNumber = e.MustBeWholeNumber;
		existing.IsActive          = e.IsActive;
		existing.IsFavorite        = e.IsFavorite;
		await _db.SaveChangesAsync();
		return Ok(existing);
	}

	[HttpDelete("{id:long}")]
	public async Task<IActionResult> Delete(long id)
	{
		var e = await _db.UOM_cs.FindAsync(id);
		if (e is null) return NotFound();
		_db.UOM_cs.Remove(e);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id:long}/toggle-favorite")]
	public async Task<IActionResult> ToggleFavorite(long id)
	{
		var e = await _db.UOM_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsFavorite = !e.IsFavorite;
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("{id:long}/toggle-active")]
	public async Task<IActionResult> ToggleActive(long id)
	{
		var e = await _db.UOM_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsActive = !e.IsActive;
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("bulk-active")]
	public async Task<IActionResult> BulkSetActive([FromBody] BulkUomActiveDto dto)
	{
		var entities = await _db.UOM_cs
			.Where(u => dto.Ids.Contains(u.Id))
			.ToListAsync();
		entities.ForEach(e => e.IsActive = dto.IsActive);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] List<long> ids)
	{
		var entities = await _db.UOM_cs
			.Where(u => ids.Contains(u.Id))
			.ToListAsync();
		_db.UOM_cs.RemoveRange(entities);
		await _db.SaveChangesAsync();
		return NoContent();
	}
}
