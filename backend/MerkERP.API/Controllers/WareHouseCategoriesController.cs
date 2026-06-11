using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.API.Services;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WareHouseCategoriesController(MerkDbContext db, ExcelService excel) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await db.WareHouseCategory_cs.OrderBy(c => c.Name_EN).ToListAsync());

	[HttpGet("nextcode")]
	public async Task<IActionResult> NextCode()
	{
		const string prefix = "WHC-";
		var codes = await db.WareHouseCategory_cs
			.Where(c => c.InternalCode != null && c.InternalCode.StartsWith(prefix))
			.Select(c => c.InternalCode!)
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
			("Name EN",       32),
			("Name AR",       32),
			("Description",   40),
		};

		return File(excel.BuildTemplate(columns),
			"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
			"warehouse-categories-template.xlsx");
	}

	[HttpPost("import")]
	public async Task<IActionResult> Import(IFormFile file)
	{
		if (file is null || file.Length == 0)
			return BadRequest("No file uploaded.");

		var created = 0;
		var errors  = new List<string>();
		var rows    = excel.ReadRows(file.OpenReadStream());

		for (int i = 0; i < rows.Count; i++)
		{
			var row          = rows[i];
			var rowNum       = i + 2;
			var internalCode = row.Length > 0 ? row[0] : "";
			var nameEn       = row.Length > 1 ? row[1] : "";
			var nameAr       = row.Length > 2 ? row[2] : "";
			var desc         = row.Length > 3 ? row[3] : "";

			if (string.IsNullOrWhiteSpace(internalCode) && string.IsNullOrWhiteSpace(nameEn)) continue;

			if (string.IsNullOrWhiteSpace(internalCode))
			{ errors.Add($"Row {rowNum}: Internal Code is required."); continue; }

			if (string.IsNullOrWhiteSpace(nameEn))
			{ errors.Add($"Row {rowNum}: Name EN is required."); continue; }

			db.WareHouseCategory_cs.Add(new WareHouseCategory_cs
			{
				InternalCode = internalCode,
				Name_EN      = nameEn,
				Name_AR      = string.IsNullOrWhiteSpace(nameAr) ? null : nameAr,
				Description  = string.IsNullOrWhiteSpace(desc)   ? null : desc,
				IsActive     = true,
			});
			created++;
		}

		if (created > 0) await db.SaveChangesAsync();
		return Ok(new { created, errors });
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> Get(long id) =>
		await db.WareHouseCategory_cs.FindAsync(id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(WareHouseCategory_cs e)
	{
		db.WareHouseCategory_cs.Add(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(long id, WareHouseCategory_cs e)
	{
		if (id != e.Id) return BadRequest();
		db.WareHouseCategory_cs.Update(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(long id)
	{
		var e = await db.WareHouseCategory_cs.FindAsync(id);
		if (e is null) return NotFound();
		db.WareHouseCategory_cs.Remove(e);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] long[] ids)
	{
		var entities = await db.WareHouseCategory_cs.Where(c => ids.Contains(c.Id)).ToListAsync();
		db.WareHouseCategory_cs.RemoveRange(entities);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id}/toggle-active")]
	public async Task<IActionResult> ToggleActive(long id)
	{
		var e = await db.WareHouseCategory_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsActive = !e.IsActive;
		await db.SaveChangesAsync();
		return Ok(e);
	}
}
