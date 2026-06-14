using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

	public StockReconciliationTransactionsController(MerkDbContext db)
	{
		_db = db;
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
