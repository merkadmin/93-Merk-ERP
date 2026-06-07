using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.API.Services;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

public record BulkItemGroupActiveDto(List<long> Ids, bool IsActive);

[ApiController]
[Route("api/[controller]")]
public class ItemGroupsController(MerkDbContext db, ExcelService excel) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await db.ItemGroup_cs.OrderBy(g => g.Name_EN).ToListAsync());

	[HttpGet("export-template")]
	public IActionResult ExportTemplate()
	{
		var columns = new (string Label, int Width)[]
		{
			("Internal Code",     18),
			("Name EN",           28),
			("Name AR",           28),
			("Parent Group Name", 28),
			("Is Main (yes/no)",  20),
		};

		return File(excel.BuildTemplate(columns),
			"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
			"item-groups-template.xlsx");
	}

	[HttpPost("import")]
	public async Task<IActionResult> Import(IFormFile file)
	{
		if (file is null || file.Length == 0)
			return BadRequest("No file uploaded.");

		var existing = await db.ItemGroup_cs.ToListAsync();
		var created  = 0;
		var errors   = new List<string>();
		var rows     = excel.ReadRows(file.OpenReadStream());

		for (int i = 0; i < rows.Count; i++)
		{
			var row          = rows[i];
			var rowNum       = i + 2;
			var internalCode = row.Length > 0 ? row[0] : "";
			var nameEN       = row.Length > 1 ? row[1] : "";
			var nameAR       = row.Length > 2 ? row[2] : "";
			var parentName   = row.Length > 3 ? row[3] : "";
			var isMainStr    = row.Length > 4 ? row[4] : "";

			if (string.IsNullOrWhiteSpace(nameEN)) continue;

			long? parentId = null;
			if (!string.IsNullOrWhiteSpace(parentName))
			{
				var parent = existing.FirstOrDefault(g =>
					string.Equals(g.Name_EN, parentName, StringComparison.OrdinalIgnoreCase) ||
					string.Equals(g.Name_AR, parentName, StringComparison.OrdinalIgnoreCase));
				if (parent is null)
				{ errors.Add($"Row {rowNum}: Parent group \"{parentName}\" not found."); continue; }
				parentId = parent.ItemGroupId;
			}

			var isMain = isMainStr.Trim().ToLowerInvariant() is "yes" or "true" or "1";

			db.ItemGroup_cs.Add(new ItemGroup_cs
			{
				InternalCode      = string.IsNullOrWhiteSpace(internalCode) ? null : internalCode,
				Name_EN           = nameEN,
				Name_AR           = string.IsNullOrWhiteSpace(nameAR) ? null : nameAR,
				ParentItemGroupId = parentId,
				IsMain            = isMain,
				IsActive          = true,
				InsertedDate      = DateTime.UtcNow,
			});
			created++;
		}

		if (created > 0) await db.SaveChangesAsync();
		return Ok(new { created, errors });
	}

	[HttpGet("{id:long}")]
	public async Task<IActionResult> Get(long id) =>
		await db.ItemGroup_cs.FindAsync(id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(ItemGroup_cs e)
	{
		e.InsertedDate = DateTime.UtcNow;
		db.ItemGroup_cs.Add(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id:long}")]
	public async Task<IActionResult> Update(long id, ItemGroup_cs e)
	{
		if (id != e.ItemGroupId) return BadRequest();
		var existing = await db.ItemGroup_cs.FindAsync(id);
		if (existing is null) return NotFound();
		existing.InternalCode      = e.InternalCode;
		existing.Name_EN           = e.Name_EN;
		existing.Name_AR           = e.Name_AR;
		existing.ParentItemGroupId = e.ParentItemGroupId;
		existing.IsMain            = e.IsMain;
		existing.IsActive          = e.IsActive;
		existing.IsFavorite        = e.IsFavorite;
		await db.SaveChangesAsync();
		return Ok(existing);
	}

	[HttpDelete("{id:long}")]
	public async Task<IActionResult> Delete(long id)
	{
		var e = await db.ItemGroup_cs.FindAsync(id);
		if (e is null) return NotFound();

		var children = await db.ItemGroup_cs
			.Where(g => g.ParentItemGroupId == id)
			.ToListAsync();
		children.ForEach(c => c.ParentItemGroupId = null);

		db.ItemGroup_cs.Remove(e);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id:long}/toggle-favorite")]
	public async Task<IActionResult> ToggleFavorite(long id)
	{
		var e = await db.ItemGroup_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsFavorite = !e.IsFavorite;
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("{id:long}/toggle-active")]
	public async Task<IActionResult> ToggleActive(long id)
	{
		var e = await db.ItemGroup_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsActive = !e.IsActive;
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("bulk-active")]
	public async Task<IActionResult> BulkSetActive([FromBody] BulkItemGroupActiveDto dto)
	{
		var entities = await db.ItemGroup_cs
			.Where(g => dto.Ids.Contains(g.ItemGroupId))
			.ToListAsync();
		entities.ForEach(e => e.IsActive = dto.IsActive);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] List<long> ids)
	{
		var entities = await db.ItemGroup_cs
			.Where(g => ids.Contains(g.ItemGroupId))
			.ToListAsync();
		db.ItemGroup_cs.RemoveRange(entities);
		await db.SaveChangesAsync();
		return NoContent();
	}
}
