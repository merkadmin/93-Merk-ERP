using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.API.Services;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

public record BulkActiveDto(List<int> Ids, bool IsActive);

[ApiController]
[Route("api/[controller]")]
public class UOMConversionGroupsController(MerkDbContext db, ExcelService excel) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await db.UOMConversionGroup_cs.ToListAsync());

	[HttpGet("nextcode")]
	public async Task<IActionResult> NextCode()
	{
		const string prefix = "UGR-";
		var codes = await db.UOMConversionGroup_cs
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

		var bytes = excel.BuildTemplate(columns);

		return File(bytes,
			"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
			"uom-conversion-groups-template.xlsx");
	}

	[HttpPost("import")]
	public async Task<IActionResult> Import(IFormFile file)
	{
		if (file is null || file.Length == 0)
			return BadRequest("No file uploaded.");

		var created = 0;
		var errors  = new List<string>();

		var rows = excel.ReadRows(file.OpenReadStream());

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

			db.UOMConversionGroup_cs.Add(new UOMConversionGroup_cs
			{
				InternalCode = internalCode,
				Name_EN      = nameEN,
				Name_AR      = nameAR,
				IsActive     = true,
				IsFavorite   = false,
				InsertedDate = DateTime.UtcNow,
			});
			created++;
		}

		if (created > 0) await db.SaveChangesAsync();

		return Ok(new { created, errors });
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> Get(int id) =>
		await db.UOMConversionGroup_cs.FindAsync(id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(UOMConversionGroup_cs e)
	{
		db.UOMConversionGroup_cs.Add(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Update(int id, UOMConversionGroup_cs e)
	{
		if (id != e.Id) return BadRequest();
		db.UOMConversionGroup_cs.Update(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
	{
		var e = await db.UOMConversionGroup_cs.FindAsync(id);
		if (e is null) return NotFound();
		db.UOMConversionGroup_cs.Remove(e);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id:int}/toggle-favorite")]
	public async Task<IActionResult> ToggleFavorite(int id)
	{
		var e = await db.UOMConversionGroup_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsFavorite = !e.IsFavorite;
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("{id:int}/toggle-active")]
	public async Task<IActionResult> ToggleActive(int id)
	{
		var e = await db.UOMConversionGroup_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsActive = !e.IsActive;
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("bulk-active")]
	public async Task<IActionResult> BulkSetActive([FromBody] BulkActiveDto dto)
	{
		var entities = await db.UOMConversionGroup_cs
			.Where(g => dto.Ids.Contains(g.Id))
			.ToListAsync();
		entities.ForEach(e => e.IsActive = dto.IsActive);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] List<int> ids)
	{
		var entities = await db.UOMConversionGroup_cs
			.Where(g => ids.Contains(g.Id))
			.ToListAsync();
		db.UOMConversionGroup_cs.RemoveRange(entities);
		await db.SaveChangesAsync();
		return NoContent();
	}
}
