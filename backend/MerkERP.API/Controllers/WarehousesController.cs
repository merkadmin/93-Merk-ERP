using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.API.Services;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehousesController : ControllerBase
{
	private readonly MerkDbContext _db;
	private readonly ExcelService  _excel;

	public WarehousesController(MerkDbContext db, ExcelService excel)
	{
		_db    = db;
		_excel = excel;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await _db.WareHouse_cs.OrderBy(w => w.Name_EN).ToListAsync());

	[HttpGet("nextcode")]
	public async Task<IActionResult> NextCode()
	{
		const string prefix = "WH-";
		var codes = await _db.WareHouse_cs
			.Where(w => w.InternalCode != null && w.InternalCode.StartsWith(prefix))
			.Select(w => w.InternalCode!)
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
		var types      = await _db.WareHouseType_s.OrderBy(t => t.Name_EN).ToListAsync();
		var categories = await _db.WareHouseCategory_cs.OrderBy(c => c.Name_EN).ToListAsync();
		var warehouses = await _db.WareHouse_cs.OrderBy(w => w.InternalCode).ToListAsync();

		var columns = new (string Label, int Width)[]
		{
			("Internal Code",        18),
			("Name EN",              32),
			("Name AR",              32),
			("Description",          40),
			("Parent Internal Code", 22),
			("Type Name EN",         24),
			("Category Name EN",     24),
			("Is Parent (yes/no)",   18),
		};

		var referenceSheets = new ReferenceSheet[]
		{
			new("WareHouseTypes",
				["Name EN", "Name AR"],
				types.Select(t => new[] { t.Name_EN, t.Name_AR ?? "" })),

			new("WareHouseCategories",
				["Internal Code", "Name EN", "Name AR"],
				categories.Select(c => new[] { c.InternalCode ?? "", c.Name_EN, c.Name_AR ?? "" })),

			new("Warehouses",
				["Internal Code", "Name EN", "Name AR"],
				warehouses.Select(w => new[] { w.InternalCode ?? "", w.Name_EN, w.Name_AR ?? "" })),
		};

		return File(_excel.BuildTemplate(columns, referenceSheets),
			"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
			"warehouses-template.xlsx");
	}

	[HttpPost("import")]
	public async Task<IActionResult> Import(IFormFile file, [FromForm] long? insertedBy = null)
	{
		if (file is null || file.Length == 0)
			return BadRequest("No file uploaded.");

		var types      = await _db.WareHouseType_s.ToListAsync();
		var categories = await _db.WareHouseCategory_cs.ToListAsync();
		var warehouses = await _db.WareHouse_cs.ToListAsync();

		var created = 0;
		var errors  = new List<string>();
		var rows    = _excel.ReadRows(file.OpenReadStream());

		for (int i = 0; i < rows.Count; i++)
		{
			var row          = rows[i];
			var rowNum       = i + 2;
			var internalCode = row.Length > 0 ? row[0] : "";
			var nameEn       = row.Length > 1 ? row[1] : "";
			var nameAr       = row.Length > 2 ? row[2] : "";
			var desc         = row.Length > 3 ? row[3] : "";
			var parentCode   = row.Length > 4 ? row[4] : "";
			var typeName     = row.Length > 5 ? row[5] : "";
			var categoryName = row.Length > 6 ? row[6] : "";
			var isParentStr  = row.Length > 7 ? row[7] : "";

			if (string.IsNullOrWhiteSpace(internalCode) && string.IsNullOrWhiteSpace(nameEn)) continue;

			if (string.IsNullOrWhiteSpace(internalCode))
			{ errors.Add($"Row {rowNum}: Internal Code is required."); continue; }

			if (string.IsNullOrWhiteSpace(nameEn))
			{ errors.Add($"Row {rowNum}: Name EN is required."); continue; }

			long? parentId = null;
			if (!string.IsNullOrWhiteSpace(parentCode))
			{
				var parent = warehouses.FirstOrDefault(w =>
					string.Equals(w.InternalCode, parentCode, StringComparison.OrdinalIgnoreCase));
				if (parent is null)
				{ errors.Add($"Row {rowNum}: Parent warehouse \"{parentCode}\" not found."); continue; }
				parentId = parent.Id;
			}
			else
			{
				var existingRoot = warehouses.FirstOrDefault(w => w.ParentWarehouseId == null);
				if (existingRoot != null)
				{ errors.Add($"Row {rowNum}: Root warehouse \"{existingRoot.InternalCode}\" already exists. Specify a Parent Internal Code."); continue; }
			}

			long? typeId = null;
			if (!string.IsNullOrWhiteSpace(typeName))
			{
				var type = types.FirstOrDefault(t =>
					string.Equals(t.Name_EN, typeName, StringComparison.OrdinalIgnoreCase) ||
					string.Equals(t.Name_AR, typeName, StringComparison.OrdinalIgnoreCase));
				if (type is null)
				{ errors.Add($"Row {rowNum}: Warehouse type \"{typeName}\" not found."); continue; }
				typeId = type.Id;
			}

			long? categoryId = null;
			if (!string.IsNullOrWhiteSpace(categoryName))
			{
				var cat = categories.FirstOrDefault(c =>
					string.Equals(c.Name_EN,      categoryName, StringComparison.OrdinalIgnoreCase) ||
					string.Equals(c.Name_AR,      categoryName, StringComparison.OrdinalIgnoreCase) ||
					string.Equals(c.InternalCode, categoryName, StringComparison.OrdinalIgnoreCase));
				if (cat is null)
				{ errors.Add($"Row {rowNum}: Warehouse category \"{categoryName}\" not found."); continue; }
				categoryId = cat.Id;
			}

			_db.WareHouse_cs.Add(new WareHouse_cs
			{
				InternalCode        = internalCode,
				Name_EN             = nameEn,
				Name_AR             = string.IsNullOrWhiteSpace(nameAr) ? null : nameAr,
				Description         = string.IsNullOrWhiteSpace(desc)   ? null : desc,
				ParentWarehouseId   = parentId,
				WareHouseTypeId     = typeId,
				WareHouseCategoryId = categoryId,
				IsParent            = isParentStr.Equals("yes", StringComparison.OrdinalIgnoreCase),
				IsActive            = true,
				InsertedBy          = insertedBy,
				InsertedDate        = DateTime.UtcNow,
			});
			created++;
		}

		if (created > 0) await _db.SaveChangesAsync();
		return Ok(new { created, errors });
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> Get(long id) =>
		await _db.WareHouse_cs.FindAsync(id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(WareHouse_cs e)
	{
		if (e.ParentWarehouseId == null)
		{
			bool rootExists = await _db.WareHouse_cs.AnyAsync(w => w.ParentWarehouseId == null);
			if (rootExists)
				return BadRequest(new { message = "A root warehouse already exists. All new warehouses must have a parent." });
		}
		_db.WareHouse_cs.Add(e);
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(long id, WareHouse_cs e)
	{
		if (id != e.Id) return BadRequest();
		if (e.ParentWarehouseId == null)
		{
			bool otherRootExists = await _db.WareHouse_cs.AnyAsync(w => w.ParentWarehouseId == null && w.Id != id);
			if (otherRootExists)
				return BadRequest(new { message = "A root warehouse already exists. Only one root warehouse is allowed." });
		}
		_db.WareHouse_cs.Update(e);
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(long id)
	{
		var e = await _db.WareHouse_cs.FindAsync(id);
		if (e is null) return NotFound();
		_db.WareHouse_cs.Remove(e);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] long[] ids)
	{
		var entities = await _db.WareHouse_cs.Where(w => ids.Contains(w.Id)).ToListAsync();
		_db.WareHouse_cs.RemoveRange(entities);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id}/toggle-active")]
	public async Task<IActionResult> ToggleActive(long id)
	{
		var e = await _db.WareHouse_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsActive = !e.IsActive;
		await _db.SaveChangesAsync();
		return Ok(e);
	}
}
