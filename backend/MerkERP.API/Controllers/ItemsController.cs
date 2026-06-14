using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.API.Services;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

public record BulkItemActiveDto(List<long> Ids, bool IsActive);

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
	private readonly MerkDbContext _db;
	private readonly ExcelService  _excel;

	public ItemsController(MerkDbContext db, ExcelService excel)
	{
		_db    = db;
		_excel = excel;
	}

	[HttpGet("nextcode")]
	public async Task<IActionResult> NextCode()
	{
		const string prefix = "ITM-";
		var codes = await _db.Item_cs
			.Where(i => i.InternalCode != null && i.InternalCode.StartsWith(prefix))
			.Select(i => i.InternalCode)
			.ToListAsync();

		var maxNum = codes
			.Select(c => int.TryParse(c[prefix.Length..], out var n) ? n : 0)
			.DefaultIfEmpty(0)
			.Max();

		return Ok(new { code = $"{prefix}{(maxNum + 1):D3}" });
	}

	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await _db.Item_cs
			.Include(i => i.ItemGroup)
			.Include(i => i.ItemType)
			.Include(i => i.DefaultUOM)
			.Include(i => i.DefaultPurchaseUOM)
			.Include(i => i.DefaultSellingUOM)
			.OrderBy(i => i.InternalCode)
			.ToListAsync());

	[HttpGet("export-template")]
	public async Task<IActionResult> ExportTemplate()
	{
		var groups = await _db.ItemGroup_cs.OrderBy(g => g.Name_EN).ToListAsync();
		var types  = await _db.ItemType_s.OrderBy(t => t.Name).ToListAsync();
		var uoms   = await _db.UOM_cs.OrderBy(u => u.Name_EN).ToListAsync();

		var columns = new (string Label, int Width)[]
		{
			("Internal Code",          18),
			("Item Name EN",           32),
			("Item Name AR",           32),
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

		return File(_excel.BuildTemplate(columns, referenceSheets),
			"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
			"items-template.xlsx");
	}

	[HttpPost("import")]
	public async Task<IActionResult> Import(IFormFile file, [FromForm] long? insertedBy = null)
	{
		if (file is null || file.Length == 0)
			return BadRequest("No file uploaded.");

		var groups  = await _db.ItemGroup_cs.ToListAsync();
		var types   = await _db.ItemType_s.ToListAsync();
		var uoms    = await _db.UOM_cs.ToListAsync();

		var created = 0;
		var errors  = new List<string>();
		var rows    = _excel.ReadRows(file.OpenReadStream());

		for (int i = 0; i < rows.Count; i++)
		{
			var row           = rows[i];
			var rowNum        = i + 2;
			var internalCode  = row.Length > 0 ? row[0] : "";
			var nameEn        = row.Length > 1 ? row[1] : "";
			var nameAr        = row.Length > 2 ? row[2] : "";
			var groupName     = row.Length > 3 ? row[3] : "";
			var typeName      = row.Length > 4 ? row[4] : "";
			var uomName       = row.Length > 5 ? row[5] : "";
			var desc          = row.Length > 6 ? row[6] : "";

			if (string.IsNullOrWhiteSpace(internalCode) && string.IsNullOrWhiteSpace(nameEn)) continue;

			if (string.IsNullOrWhiteSpace(internalCode))
			{ errors.Add($"Row {rowNum}: Internal Code is required."); continue; }

			if (string.IsNullOrWhiteSpace(nameEn))
			{ errors.Add($"Row {rowNum}: Item Name EN is required."); continue; }

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

			_db.Item_cs.Add(new Item_cs
			{
				InternalCode = internalCode,
				Name_EN      = nameEn,
				Name_AR      = string.IsNullOrWhiteSpace(nameAr) ? null : nameAr,
				ItemGroupId  = group?.ItemGroupId ?? 0,
				ItemTypeId   = type.ItemTypeId,
				DefaultUOMId = uom.Id,
				Description  = string.IsNullOrWhiteSpace(desc) ? null : desc,
				IsActive     = true,
				InsertedBy   = insertedBy,
				InsertedDate = DateTime.UtcNow,
			});
			created++;
		}

		if (created > 0) await _db.SaveChangesAsync();
		return Ok(new { created, errors });
	}

	[HttpGet("{id:long}")]
	public async Task<IActionResult> Get(long id) =>
		await _db.Item_cs
			.Include(i => i.ItemGroup)
			.Include(i => i.ItemType)
			.Include(i => i.DefaultUOM)
			.Include(i => i.DefaultPurchaseUOM)
			.Include(i => i.DefaultSellingUOM)
			.FirstOrDefaultAsync(i => i.Id == id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(Item_cs e)
	{
		e.InsertedDate = DateTime.UtcNow;
		_db.Item_cs.Add(e);
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id:long}")]
	public async Task<IActionResult> Update(long id, Item_cs e)
	{
		if (id != e.Id) return BadRequest();
		var existing = await _db.Item_cs.FindAsync(id);
		if (existing is null) return NotFound();
		existing.InternalCode          = e.InternalCode;
		existing.Name_EN               = e.Name_EN;
		existing.Name_AR               = e.Name_AR;
		existing.ItemGroupId           = e.ItemGroupId;
		existing.ItemTypeId            = e.ItemTypeId;
		existing.DefaultUOMId          = e.DefaultUOMId;
		existing.DefaultPurchaseUOMId  = e.DefaultPurchaseUOMId;
		existing.AcceptSelling         = e.AcceptSelling;
		existing.DefaultSellingUOMId   = e.DefaultSellingUOMId;
		existing.Description           = e.Description;
		existing.OpeningStock          = e.OpeningStock;
		existing.ExpirationDate        = e.ExpirationDate;
		existing.MinOrderQuantity      = e.MinOrderQuantity;
		existing.SafetyStock           = e.SafetyStock;
		existing.IsActive              = e.IsActive;
		existing.IsFavorite            = e.IsFavorite;
		await _db.SaveChangesAsync();
		return Ok(existing);
	}

	[HttpDelete("{id:long}")]
	public async Task<IActionResult> Delete(long id)
	{
		var e = await _db.Item_cs.FindAsync(id);
		if (e is null) return NotFound();
		_db.Item_cs.Remove(e);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id:long}/toggle-favorite")]
	public async Task<IActionResult> ToggleFavorite(long id)
	{
		var e = await _db.Item_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsFavorite = !e.IsFavorite;
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("{id:long}/toggle-active")]
	public async Task<IActionResult> ToggleActive(long id)
	{
		var e = await _db.Item_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsActive = !e.IsActive;
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("bulk-active")]
	public async Task<IActionResult> BulkSetActive([FromBody] BulkItemActiveDto dto)
	{
		var entities = await _db.Item_cs
			.Where(i => dto.Ids.Contains(i.Id))
			.ToListAsync();
		entities.ForEach(e => e.IsActive = dto.IsActive);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] List<long> ids)
	{
		var entities = await _db.Item_cs
			.Where(i => ids.Contains(i.Id))
			.ToListAsync();
		_db.Item_cs.RemoveRange(entities);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	// ── Barcode lookup (global) ─────────────────────────────────────────────

	[HttpGet("barcode/{code}")]
	public async Task<IActionResult> GetByBarcode(string code)
	{
		var b = await _db.Item_UOM_Barcode_cs
			.Include(b => b.Item)
			.Include(b => b.UOM)
			.FirstOrDefaultAsync(b => b.Barcode == code);
		if (b is null) return NotFound();
		return Ok(new {
			itemId    = b.ItemId,
			uomId     = b.UOMId,
			itemNameEN = b.Item!.Name_EN,
			itemNameAR = b.Item!.Name_AR,
			uomNameEN  = b.UOM!.Name_EN,
			uomNameAR  = b.UOM!.Name_AR,
		});
	}

	// ── Per-item barcodes ────────────────────────────────────────────────────

	[HttpGet("{itemId:long}/barcodes")]
	public async Task<IActionResult> GetBarcodes(long itemId) =>
		Ok(await _db.Item_UOM_Barcode_cs
			.Where(b => b.ItemId == itemId)
			.Include(b => b.BarcodeType)
			.Include(b => b.UOM)
			.ToListAsync());

	[HttpPost("{itemId:long}/barcodes")]
	public async Task<IActionResult> AddBarcode(long itemId, Item_UOM_Barcode_cs barcode)
	{
		var existing = await _db.Item_UOM_Barcode_cs
			.Where(b => b.Barcode == barcode.Barcode)
			.Select(b => new { b.ItemId })
			.FirstOrDefaultAsync();

		if (existing != null)
		{
			var isSameItem = existing.ItemId == itemId;
			return Conflict(new
			{
				message = isSameItem
					? "This barcode is already assigned to this item."
					: "This barcode is already used by another item."
			});
		}

		barcode.ItemId = itemId;
		_db.Item_UOM_Barcode_cs.Add(barcode);
		await _db.SaveChangesAsync();
		return Ok(barcode);
	}

	[HttpDelete("{itemId:long}/barcodes/{id:long}")]
	public async Task<IActionResult> DeleteBarcode(long itemId, long id)
	{
		var b = await _db.Item_UOM_Barcode_cs.FindAsync(id);
		if (b is null || b.ItemId != itemId) return NotFound();
		_db.Item_UOM_Barcode_cs.Remove(b);
		await _db.SaveChangesAsync();
		return NoContent();
	}
}
