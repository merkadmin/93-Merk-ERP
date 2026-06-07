using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.API.Services;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

public record BulkItemActiveDto(List<long> Ids, bool IsActive);

[ApiController]
[Route("api/[controller]")]
public class ItemsController(MerkDbContext db, ExcelService excel) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await db.Item_cs
			.Include(i => i.ItemGroup)
			.Include(i => i.ItemType)
			.Include(i => i.DefaultUOM)
			.OrderBy(i => i.ItemCode)
			.ToListAsync());

	[HttpGet("export-template")]
	public async Task<IActionResult> ExportTemplate()
	{
		var groups = await db.ItemGroup_cs.OrderBy(g => g.Name_EN).ToListAsync();
		var types  = await db.ItemType_s.OrderBy(t => t.Name).ToListAsync();
		var uoms   = await db.UOM_cs.OrderBy(u => u.Name_EN).ToListAsync();

		var columns = new (string Label, int Width)[]
		{
			("Item Code",              18),
			("Item Name",              32),
			("Item Group Name",        28),
			("Item Type Name",         22),
			("Default UOM (Name EN)",  22),
			("Description",            36),
			("Has Batch (yes/no)",     18),
			("Has Serial (yes/no)",    18),
		};

		var referenceSheets = new ReferenceSheet[]
		{
			new("ItemGroups",
				["Name EN", "Name AR", "Parent Group"],
				groups.Select(g => new[]
				{
					g.Name_EN,
					g.Name_AR ?? "",
					groups.FirstOrDefault(p => p.ItemGroupId == g.ParentItemGroupId)?.Name_EN ?? "",
				})),

			new("ItemTypes",
				["Name"],
				types.Select(t => new[] { t.Name })),

			new("UOMs",
				["Internal Code", "Name EN", "Name AR"],
				uoms.Select(u => new[] { u.InternalCode ?? "", u.Name_EN, u.Name_AR ?? "" })),
		};

		return File(excel.BuildTemplate(columns, referenceSheets),
			"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
			"items-template.xlsx");
	}

	[HttpPost("import")]
	public async Task<IActionResult> Import(IFormFile file)
	{
		if (file is null || file.Length == 0)
			return BadRequest("No file uploaded.");

		var groups  = await db.ItemGroup_cs.ToListAsync();
		var types   = await db.ItemType_s.ToListAsync();
		var uoms    = await db.UOM_cs.ToListAsync();

		var created = 0;
		var errors  = new List<string>();
		var rows    = excel.ReadRows(file.OpenReadStream());

		for (int i = 0; i < rows.Count; i++)
		{
			var row       = rows[i];
			var rowNum    = i + 2;
			var itemCode  = row.Length > 0 ? row[0] : "";
			var itemName  = row.Length > 1 ? row[1] : "";
			var groupName = row.Length > 2 ? row[2] : "";
			var typeName  = row.Length > 3 ? row[3] : "";
			var uomName   = row.Length > 4 ? row[4] : "";
			var desc      = row.Length > 5 ? row[5] : "";
			var batchStr  = row.Length > 6 ? row[6] : "";
			var serialStr = row.Length > 7 ? row[7] : "";

			if (string.IsNullOrWhiteSpace(itemCode) && string.IsNullOrWhiteSpace(itemName)) continue;

			if (string.IsNullOrWhiteSpace(itemCode))
			{ errors.Add($"Row {rowNum}: Item Code is required."); continue; }

			if (string.IsNullOrWhiteSpace(itemName))
			{ errors.Add($"Row {rowNum}: Item Name is required."); continue; }

			var group = groups.FirstOrDefault(g =>
				string.Equals(g.Name_EN, groupName, StringComparison.OrdinalIgnoreCase) ||
				string.Equals(g.Name_AR, groupName, StringComparison.OrdinalIgnoreCase));
			if (group is null && !string.IsNullOrWhiteSpace(groupName))
			{ errors.Add($"Row {rowNum}: Item Group \"{groupName}\" not found."); continue; }

			var type = types.FirstOrDefault(t =>
				string.Equals(t.Name, typeName, StringComparison.OrdinalIgnoreCase))
				?? types.FirstOrDefault();
			if (type is null)
			{ errors.Add($"Row {rowNum}: No item types exist in the system."); continue; }

			var uom = uoms.FirstOrDefault(u =>
				string.Equals(u.Name_EN,      uomName, StringComparison.OrdinalIgnoreCase) ||
				string.Equals(u.Name_AR,      uomName, StringComparison.OrdinalIgnoreCase) ||
				string.Equals(u.InternalCode, uomName, StringComparison.OrdinalIgnoreCase));
			if (uom is null && !string.IsNullOrWhiteSpace(uomName))
			{ errors.Add($"Row {rowNum}: UOM \"{uomName}\" not found."); continue; }
			if (uom is null)
			{ errors.Add($"Row {rowNum}: Default UOM is required."); continue; }

			db.Item_cs.Add(new Item_cs
			{
				ItemCode     = itemCode,
				ItemName     = itemName,
				ItemGroupId  = group?.ItemGroupId ?? 0,
				ItemTypeId   = type.ItemTypeId,
				DefaultUOMId = uom.Id,
				Description  = string.IsNullOrWhiteSpace(desc) ? null : desc,
				HasBatch     = batchStr.Trim().ToLowerInvariant()  is "yes" or "true" or "1",
				HasSerial    = serialStr.Trim().ToLowerInvariant() is "yes" or "true" or "1",
				IsActive     = true,
				InsertedDate = DateTime.UtcNow,
			});
			created++;
		}

		if (created > 0) await db.SaveChangesAsync();
		return Ok(new { created, errors });
	}

	[HttpGet("{id:long}")]
	public async Task<IActionResult> Get(long id) =>
		await db.Item_cs
			.Include(i => i.ItemGroup)
			.Include(i => i.ItemType)
			.Include(i => i.DefaultUOM)
			.FirstOrDefaultAsync(i => i.ItemId == id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(Item_cs e)
	{
		e.InsertedDate = DateTime.UtcNow;
		db.Item_cs.Add(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id:long}")]
	public async Task<IActionResult> Update(long id, Item_cs e)
	{
		if (id != e.ItemId) return BadRequest();
		var existing = await db.Item_cs.FindAsync(id);
		if (existing is null) return NotFound();
		existing.ItemCode     = e.ItemCode;
		existing.ItemName     = e.ItemName;
		existing.ItemGroupId  = e.ItemGroupId;
		existing.ItemTypeId   = e.ItemTypeId;
		existing.DefaultUOMId = e.DefaultUOMId;
		existing.Description  = e.Description;
		existing.HasBatch     = e.HasBatch;
		existing.HasSerial    = e.HasSerial;
		existing.IsActive     = e.IsActive;
		existing.IsFavorite   = e.IsFavorite;
		await db.SaveChangesAsync();
		return Ok(existing);
	}

	[HttpDelete("{id:long}")]
	public async Task<IActionResult> Delete(long id)
	{
		var e = await db.Item_cs.FindAsync(id);
		if (e is null) return NotFound();
		db.Item_cs.Remove(e);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id:long}/toggle-favorite")]
	public async Task<IActionResult> ToggleFavorite(long id)
	{
		var e = await db.Item_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsFavorite = !e.IsFavorite;
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("{id:long}/toggle-active")]
	public async Task<IActionResult> ToggleActive(long id)
	{
		var e = await db.Item_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsActive = !e.IsActive;
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("bulk-active")]
	public async Task<IActionResult> BulkSetActive([FromBody] BulkItemActiveDto dto)
	{
		var entities = await db.Item_cs
			.Where(i => dto.Ids.Contains(i.ItemId))
			.ToListAsync();
		entities.ForEach(e => e.IsActive = dto.IsActive);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] List<long> ids)
	{
		var entities = await db.Item_cs
			.Where(i => ids.Contains(i.ItemId))
			.ToListAsync();
		db.Item_cs.RemoveRange(entities);
		await db.SaveChangesAsync();
		return NoContent();
	}
}
