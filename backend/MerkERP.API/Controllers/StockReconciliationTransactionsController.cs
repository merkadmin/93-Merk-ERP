using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.API.Services;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

public record CreateSRTDetailDto(long ItemId, long WarehouseId, decimal Quantity, long UOMId);

public record CreateSRTDto(
	long StockTransactionTypeId,
	long StockTransactionStatusId,
	string? InternalCode,
	DateOnly PostingDate,
	TimeOnly PostingTime,
	long? SetWarehouseId,
	long? InsertedBy,
	List<CreateSRTDetailDto> Details
);

public record UpdateSRTDto(
	long StockTransactionTypeId,
	long StockTransactionStatusId,
	string? InternalCode,
	DateOnly PostingDate,
	TimeOnly PostingTime,
	long? SetWarehouseId
);

public record SkippedDetailDto(long ItemId, string ItemName, long UOMId, string UOMName, string Reason);

[ApiController]
[Route("api/[controller]")]
public class StockReconciliationTransactionsController : ControllerBase
{
	private readonly MerkDbContext _db;
	private readonly ExcelService  _excel;

	public StockReconciliationTransactionsController(MerkDbContext db, ExcelService excel)
	{
		_db    = db;
		_excel = excel;
	}

	// ── UOM conversion helper ─────────────────────────────────────────────────

	private record ConversionResult(decimal ConvertedQty, long DefaultUOMId, bool Found);

	private ConversionResult ResolveConversion(
		long fromUOMId, decimal quantity, long defaultUOMId,
		List<UOMConversionFactor_cs> factors)
	{
		if (fromUOMId == defaultUOMId)
			return new(quantity, defaultUOMId, true);

		var direct = factors.FirstOrDefault(f => f.UOMFromId == fromUOMId && f.UOMToId == defaultUOMId);
		if (direct is not null)
			return new((decimal)(quantity * (decimal)direct.Value), defaultUOMId, true);

		var inverse = factors.FirstOrDefault(f => f.UOMFromId == defaultUOMId && f.UOMToId == fromUOMId);
		if (inverse is not null && inverse.Value != 0)
			return new((decimal)(quantity / (decimal)inverse.Value), defaultUOMId, true);

		return new(quantity, fromUOMId, false);
	}

	// ── Endpoints ─────────────────────────────────────────────────────────────

	[HttpGet("export-template")]
	public async Task<IActionResult> ExportTemplate()
	{
		var txnTypes   = await _db.StockTransactionType_s.OrderBy(t => t.Name_EN).ToListAsync();
		var statuses   = await _db.StockTransactionStatus_s.OrderBy(s => s.Name_EN).ToListAsync();
		var warehouses = await _db.WareHouse_cs.OrderBy(w => w.Name_EN).ToListAsync();
		var items      = await _db.Item_cs.OrderBy(i => i.InternalCode).ToListAsync();
		var uoms       = await _db.UOM_cs.OrderBy(u => u.Name_EN).ToListAsync();

		var columns = new (string Label, int Width)[]
		{
			("Internal Code",     22),
			("Transaction Type",  24),
			("Status",            20),
			("Warehouse",         24),
			("Posting Date",      18),
			("Item Code",         20),
			("Quantity",          16),
			("UOM",               18),
		};

		var referenceSheets = new[]
		{
			new ReferenceSheet("Transaction Types",
				new[] { "Name EN", "Name AR" },
				txnTypes.Select(t => new[] { t.Name_EN, t.Name_AR ?? "" })),
			new ReferenceSheet("Statuses",
				new[] { "Name EN", "Name AR" },
				statuses.Select(s => new[] { s.Name_EN, s.Name_AR ?? "" })),
			new ReferenceSheet("Warehouses",
				new[] { "Code", "Name EN", "Name AR" },
				warehouses.Select(w => new[] { w.InternalCode ?? "", w.Name_EN, w.Name_AR ?? "" })),
			new ReferenceSheet("Items",
				new[] { "Code", "Name EN", "Name AR" },
				items.Select(i => new[] { i.InternalCode ?? "", i.Name_EN, i.Name_AR ?? "" })),
			new ReferenceSheet("UOMs",
				new[] { "Code", "Name EN", "Name AR" },
				uoms.Select(u => new[] { u.InternalCode ?? "", u.Name_EN, u.Name_AR ?? "" })),
		};

		return File(_excel.BuildTemplate(columns, referenceSheets),
			"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
			"stock-reconciliation-template.xlsx");
	}

	[HttpPost("import")]
	public async Task<IActionResult> Import(IFormFile file, [FromForm] long? insertedBy = null)
	{
		if (file is null || file.Length == 0)
			return BadRequest("No file uploaded.");

		var txnTypes   = await _db.StockTransactionType_s.ToListAsync();
		var statuses   = await _db.StockTransactionStatus_s.ToListAsync();
		var warehouses = await _db.WareHouse_cs.ToListAsync();
		var items      = await _db.Item_cs.ToListAsync();
		var uoms       = await _db.UOM_cs.ToListAsync();

		var rows    = _excel.ReadRows(file.OpenReadStream());
		var created = 0;
		var errors  = new List<string>();

		// group rows by internal code so multiple detail rows share one transaction header
		var groups = rows
			.Select((r, i) => (row: r, rowNum: i + 2))
			.Where(x => x.row.Any(c => !string.IsNullOrWhiteSpace(c)))
			.GroupBy(x => x.row.Length > 0 ? x.row[0].Trim() : "")
			.ToList();

		foreach (var grp in groups)
		{
			var firstRow    = grp.First();
			var rowNum      = firstRow.rowNum;
			var internalCode = grp.Key;

			var typeName     = firstRow.row.Length > 1 ? firstRow.row[1] : "";
			var statusName   = firstRow.row.Length > 2 ? firstRow.row[2] : "";
			var whName       = firstRow.row.Length > 3 ? firstRow.row[3] : "";
			var dateStr      = firstRow.row.Length > 4 ? firstRow.row[4] : "";

			var txnType = txnTypes.FirstOrDefault(t =>
				string.Equals(t.Name_EN, typeName, StringComparison.OrdinalIgnoreCase) ||
				string.Equals(t.Name_AR, typeName, StringComparison.OrdinalIgnoreCase));
			if (txnType is null)
			{
				errors.Add($"Row {rowNum}: Transaction Type \"{typeName}\" not found.");
				continue;
			}

			var status = statuses.FirstOrDefault(s =>
				string.Equals(s.Name_EN, statusName, StringComparison.OrdinalIgnoreCase) ||
				string.Equals(s.Name_AR, statusName, StringComparison.OrdinalIgnoreCase))
				?? statuses.FirstOrDefault();
			if (status is null) { errors.Add($"Row {rowNum}: No statuses in the system."); continue; }

			var warehouse = warehouses.FirstOrDefault(w =>
				string.Equals(w.Name_EN,      whName, StringComparison.OrdinalIgnoreCase) ||
				string.Equals(w.Name_AR,      whName, StringComparison.OrdinalIgnoreCase) ||
				string.Equals(w.InternalCode, whName, StringComparison.OrdinalIgnoreCase));
			if (warehouse is null && !string.IsNullOrWhiteSpace(whName))
			{ errors.Add($"Row {rowNum}: Warehouse \"{whName}\" not found."); continue; }

			if (!DateOnly.TryParse(dateStr, out var postingDate))
				postingDate = DateOnly.FromDateTime(DateTime.Today);

			var txn = new StockReconciliationTransaction
			{
				InternalCode             = string.IsNullOrWhiteSpace(internalCode) ? null : internalCode,
				StockTransactionTypeId   = txnType.Id,
				StockTransactionStatusId = status.Id,
				SetWarehouseId           = warehouse?.Id,
				PostingDate              = postingDate,
				PostingTime              = TimeOnly.FromDateTime(DateTime.UtcNow),
				InsertedBy               = insertedBy,
				InsertedDate             = DateTime.UtcNow,
			};

			// build details — all rows in the group
			var allUOMIdsNeeded = new List<long>();
			var rowDetails = new List<(int rowNum, Item_cs item, decimal qty, long uomId)>();

			foreach (var (row, rn) in grp)
			{
				var itemCode = row.Length > 5 ? row[5] : "";
				var qtyStr   = row.Length > 6 ? row[6] : "";
				var uomName  = row.Length > 7 ? row[7] : "";

				if (string.IsNullOrWhiteSpace(itemCode)) continue;

				var item = items.FirstOrDefault(i =>
					string.Equals(i.InternalCode, itemCode, StringComparison.OrdinalIgnoreCase) ||
					string.Equals(i.Name_EN,      itemCode, StringComparison.OrdinalIgnoreCase));
				if (item is null) { errors.Add($"Row {rn}: Item \"{itemCode}\" not found."); continue; }

				if (!decimal.TryParse(qtyStr, out var qty) || qty <= 0)
				{ errors.Add($"Row {rn}: Invalid quantity \"{qtyStr}\"."); continue; }

				var uom = uoms.FirstOrDefault(u =>
					string.Equals(u.Name_EN,      uomName, StringComparison.OrdinalIgnoreCase) ||
					string.Equals(u.Name_AR,      uomName, StringComparison.OrdinalIgnoreCase) ||
					string.Equals(u.InternalCode, uomName, StringComparison.OrdinalIgnoreCase))
					?? uoms.FirstOrDefault(u => u.Id == item.DefaultUOMId);
				if (uom is null) { errors.Add($"Row {rn}: UOM \"{uomName}\" not found."); continue; }

				allUOMIdsNeeded.Add(uom.Id);
				allUOMIdsNeeded.Add(item.DefaultUOMId);
				rowDetails.Add((rn, item, qty, uom.Id));
			}

			var distinctUOMIds = allUOMIdsNeeded.Distinct().ToList();
			var factors = await _db.UOMConversionFactor_cs
				.Where(f => f.IsActive &&
							distinctUOMIds.Contains(f.UOMFromId) &&
							distinctUOMIds.Contains(f.UOMToId))
				.ToListAsync();

			foreach (var (rn, item, qty, uomId) in rowDetails)
			{
				var conv = ResolveConversion(uomId, qty, item.DefaultUOMId, factors);
				if (!conv.Found)
				{
					errors.Add($"Row {rn}: No UOM conversion from \"{uomId}\" to item default UOM for \"{item.Name_EN}\".");
					continue;
				}
				txn.Details.Add(new StockReconciliationTransactionDetail
				{
					ItemId      = item.Id,
					WarehouseId = warehouse?.Id ?? 0,
					Quantity    = conv.ConvertedQty,
					UOMId       = conv.DefaultUOMId,
					InsertedBy  = insertedBy,
					InsertedDate = DateTime.UtcNow,
				});
			}

			_db.StockReconciliationTransaction.Add(txn);
			created++;
		}

		if (created > 0) await _db.SaveChangesAsync();
		return Ok(new { created, errors });
	}

	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await _db.StockReconciliationTransaction
			.Include(t => t.StockTransactionType)
			.Include(t => t.StockTransactionStatus)
			.Include(t => t.SetWarehouse)
			.OrderByDescending(t => t.PostingDate)
			.ThenByDescending(t => t.Id)
			.ToListAsync());

	[HttpGet("nextcode")]
	public async Task<IActionResult> NextCode()
	{
		const string prefix = "SRC-";
		var codes = await _db.StockReconciliationTransaction
			.Where(t => t.InternalCode != null && t.InternalCode.StartsWith(prefix))
			.Select(t => t.InternalCode)
			.ToListAsync();
		var max = codes
			.Select(c => int.TryParse(c![prefix.Length..], out var n) ? n : 0)
			.DefaultIfEmpty(0).Max();
		return Ok(new { code = $"{prefix}{(max + 1):D4}" });
	}

	[HttpGet("{id:long}")]
	public async Task<IActionResult> Get(long id)
	{
		var entity = await _db.StockReconciliationTransaction
			.Include(t => t.StockTransactionType)
			.Include(t => t.StockTransactionStatus)
			.Include(t => t.SetWarehouse)
			.Include(t => t.Details).ThenInclude(d => d.Item)
			.Include(t => t.Details).ThenInclude(d => d.Warehouse)
			.Include(t => t.Details).ThenInclude(d => d.UOM)
			.FirstOrDefaultAsync(t => t.Id == id);
		return entity is null ? NotFound() : Ok(entity);
	}

	[HttpPost]
	public async Task<IActionResult> Create(CreateSRTDto dto)
	{
		var itemIds = dto.Details.Select(d => d.ItemId).Distinct().ToList();
		var uomIds = dto.Details.Select(d => d.UOMId).Distinct().ToList();

		var items = await _db.Item_cs
			.Where(i => itemIds.Contains(i.Id))
			.Select(i => new { i.Id, i.DefaultUOMId, i.Name_EN, i.Name_AR })
			.ToListAsync();

		var allUOMIds = itemIds
			.Select(id => items.FirstOrDefault(i => i.Id == id)?.DefaultUOMId ?? 0)
			.Concat(uomIds)
			.Distinct()
			.ToList();

		var factors = await _db.UOMConversionFactor_cs
			.Where(f => f.IsActive &&
						allUOMIds.Contains(f.UOMFromId) &&
						allUOMIds.Contains(f.UOMToId))
			.ToListAsync();

		var uoms = await _db.UOM_cs
			.Where(u => uomIds.Contains(u.Id))
			.Select(u => new { u.Id, u.Name_EN, u.Name_AR })
			.ToListAsync();

		var savedDetails = new List<StockReconciliationTransactionDetail>();
		var skipped = new List<SkippedDetailDto>();

		foreach (var d in dto.Details)
		{
			var item = items.FirstOrDefault(i => i.Id == d.ItemId);
			var defaultUOM = item?.DefaultUOMId ?? d.UOMId;
			var conv = ResolveConversion(d.UOMId, d.Quantity, defaultUOM, factors);

			if (!conv.Found)
			{
				var uom = uoms.FirstOrDefault(u => u.Id == d.UOMId);
				skipped.Add(new SkippedDetailDto(
					d.ItemId,
					item?.Name_EN ?? d.ItemId.ToString(),
					d.UOMId,
					uom?.Name_EN ?? d.UOMId.ToString(),
					"No UOM conversion found to item default UOM"
				));
				continue;
			}

			savedDetails.Add(new StockReconciliationTransactionDetail
			{
				ItemId = d.ItemId,
				WarehouseId = d.WarehouseId,
				Quantity = conv.ConvertedQty,
				UOMId = conv.DefaultUOMId,
				InsertedBy = dto.InsertedBy,
				InsertedDate = DateTime.UtcNow,
			});
		}

		var entity = new StockReconciliationTransaction
		{
			StockTransactionTypeId = dto.StockTransactionTypeId,
			StockTransactionStatusId = dto.StockTransactionStatusId,
			InternalCode = dto.InternalCode,
			PostingDate = dto.PostingDate,
			PostingTime = dto.PostingTime,
			SetWarehouseId = dto.SetWarehouseId,
			InsertedBy = dto.InsertedBy,
			InsertedDate = DateTime.UtcNow,
			Details = savedDetails,
		};

		_db.StockReconciliationTransaction.Add(entity);
		await _db.SaveChangesAsync();

		return Ok(new { entity, skipped });
	}

	[HttpPut("{id:long}")]
	public async Task<IActionResult> Update(long id, UpdateSRTDto dto)
	{
		var entity = await _db.StockReconciliationTransaction.FindAsync(id);
		if (entity is null) return NotFound();
		entity.StockTransactionTypeId = dto.StockTransactionTypeId;
		entity.StockTransactionStatusId = dto.StockTransactionStatusId;
		entity.InternalCode = dto.InternalCode;
		entity.PostingDate = dto.PostingDate;
		entity.PostingTime = dto.PostingTime;
		entity.SetWarehouseId = dto.SetWarehouseId;
		await _db.SaveChangesAsync();
		return Ok(entity);
	}

	[HttpDelete("{id:long}")]
	public async Task<IActionResult> Delete(long id)
	{
		var entity = await _db.StockReconciliationTransaction.FindAsync(id);
		if (entity is null) return NotFound();
		_db.StockReconciliationTransaction.Remove(entity);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id:long}/submit")]
	public async Task<IActionResult> Submit(long id)
	{
		var entity = await _db.StockReconciliationTransaction.FindAsync(id);
		if (entity is null) return NotFound();
		entity.StockTransactionStatusId = 3; // Submitted
		await _db.SaveChangesAsync();
		return Ok(entity);
	}

	[HttpPatch("{id:long}/reissue")]
	public async Task<IActionResult> Reissue(long id)
	{
		var entity = await _db.StockReconciliationTransaction.FindAsync(id);
		if (entity is null) return NotFound();
		entity.StockTransactionStatusId = 1; // Draft
		await _db.SaveChangesAsync();
		return Ok(entity);
	}

	[HttpPatch("{id:long}/cancel")]
	public async Task<IActionResult> CancelTransaction(long id)
	{
		var entity = await _db.StockReconciliationTransaction.FindAsync(id);
		if (entity is null) return NotFound();
		entity.StockTransactionStatusId = 4; // Cancelled
		await _db.SaveChangesAsync();
		return Ok(entity);
	}

	[HttpPatch("{id:long}/toggle-active")]
	public async Task<IActionResult> ToggleActive(long id)
	{
		var entity = await _db.StockReconciliationTransaction.FindAsync(id);
		if (entity is null) return NotFound();
		entity.IsActive = !entity.IsActive;
		await _db.SaveChangesAsync();
		return Ok(entity);
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] List<long> ids)
	{
		var entities = await _db.StockReconciliationTransaction
			.Where(t => ids.Contains(t.Id)).ToListAsync();
		_db.StockReconciliationTransaction.RemoveRange(entities);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPost("{id:long}/details")]
	public async Task<IActionResult> AddDetail(long id, CreateSRTDetailDto dto)
	{
		var exists = await _db.StockReconciliationTransaction.AnyAsync(t => t.Id == id);
		if (!exists) return NotFound();

		var item = await _db.Item_cs
			.Where(i => i.Id == dto.ItemId)
			.Select(i => new { i.DefaultUOMId, i.Name_EN, i.Name_AR })
			.FirstOrDefaultAsync();

		var defaultUOM = item?.DefaultUOMId ?? dto.UOMId;

		var allUOMIds = new[] { dto.UOMId, defaultUOM }.Distinct().ToList();
		var factors = await _db.UOMConversionFactor_cs
			.Where(f => f.IsActive &&
						allUOMIds.Contains(f.UOMFromId) &&
						allUOMIds.Contains(f.UOMToId))
			.ToListAsync();

		var conv = ResolveConversion(dto.UOMId, dto.Quantity, defaultUOM, factors);

		if (!conv.Found)
		{
			var uom = await _db.UOM_cs
				.Where(u => u.Id == dto.UOMId)
				.Select(u => new { u.Name_EN })
				.FirstOrDefaultAsync();

			return Ok(new
			{
				saved = (object?)null,
				skipped = new[]
				{
					new SkippedDetailDto(
						dto.ItemId,
						item?.Name_EN ?? dto.ItemId.ToString(),
						dto.UOMId,
						uom?.Name_EN ?? dto.UOMId.ToString(),
						"No UOM conversion found to item default UOM"
					)
				}
			});
		}

		var detail = new StockReconciliationTransactionDetail
		{
			StockReconciliationTransactionId = id,
			ItemId = dto.ItemId,
			WarehouseId = dto.WarehouseId,
			Quantity = conv.ConvertedQty,
			UOMId = conv.DefaultUOMId,
			InsertedDate = DateTime.UtcNow,
		};
		_db.StockReconciliationTransactionDetail.Add(detail);
		await _db.SaveChangesAsync();

		return Ok(new { saved = detail, skipped = Array.Empty<SkippedDetailDto>() });
	}

	[HttpDelete("{id:long}/details/{detailId:long}")]
	public async Task<IActionResult> RemoveDetail(long id, long detailId)
	{
		var detail = await _db.StockReconciliationTransactionDetail
			.FirstOrDefaultAsync(d => d.Id == detailId && d.StockReconciliationTransactionId == id);
		if (detail is null) return NotFound();
		_db.StockReconciliationTransactionDetail.Remove(detail);
		await _db.SaveChangesAsync();
		return NoContent();
	}
}
