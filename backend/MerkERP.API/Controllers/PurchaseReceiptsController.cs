using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

public record CreatePRItemDto(long ItemId, long UOMId, decimal Quantity, decimal Rate);
public record CreatePRTaxDto(string Description, decimal? Rate, decimal Amount);

public record CreatePRDto(
	string? InternalCode,
	long SupplierId,
	long CompanyId,
	long CurrencyId,
	string? SupplierDeliveryNote,
	DateOnly PostingDate,
	TimeOnly PostingTime,
	long? SetWarehouseId,
	string? Remarks,
	long? InsertedBy,
	List<CreatePRItemDto> Items,
	List<CreatePRTaxDto> Taxes
);

public record UpdatePRDto(
	string? InternalCode,
	long SupplierId,
	long CompanyId,
	long CurrencyId,
	string? SupplierDeliveryNote,
	DateOnly PostingDate,
	TimeOnly PostingTime,
	long? SetWarehouseId,
	string? Remarks
);

[ApiController]
[Route("api/[controller]")]
public class PurchaseReceiptsController : ControllerBase
{
	private readonly MerkDbContext _db;

	public PurchaseReceiptsController(MerkDbContext db)
	{
		_db = db;
	}

	// ── Totals helper ────────────────────────────────────────────────────────

	private async Task RecalculateTotalsAsync(long purchaseReceiptId)
	{
		var entity = await _db.PurchaseReceipt.FindAsync(purchaseReceiptId);
		if (entity is null) return;

		var items = await _db.PurchaseReceiptItem.Where(i => i.PurchaseReceiptId == purchaseReceiptId).ToListAsync();
		var taxes = await _db.PurchaseReceiptTax.Where(t => t.PurchaseReceiptId == purchaseReceiptId).ToListAsync();

		entity.TotalQty = items.Sum(i => i.Quantity);
		entity.Total = items.Sum(i => i.Amount);
		entity.TaxTotal = taxes.Sum(t => t.Amount);
		entity.GrandTotal = entity.Total + entity.TaxTotal;

		await _db.SaveChangesAsync();
	}

	// ── Endpoints ─────────────────────────────────────────────────────────────

	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await _db.PurchaseReceipt
			.Include(p => p.Supplier)
			.Include(p => p.Company)
			.Include(p => p.SetWarehouse)
			.Include(p => p.StockTransactionStatus)
			.OrderByDescending(p => p.PostingDate)
			.ThenByDescending(p => p.Id)
			.ToListAsync());

	[HttpGet("nextcode")]
	public async Task<IActionResult> NextCode()
	{
		const string prefix = "PR-";
		var codes = await _db.PurchaseReceipt
			.Where(p => p.InternalCode != null && p.InternalCode.StartsWith(prefix))
			.Select(p => p.InternalCode!)
			.ToListAsync();

		var maxNum = codes
			.Select(c => int.TryParse(c[prefix.Length..], out var n) ? n : 0)
			.DefaultIfEmpty(0)
			.Max();

		return Ok(new { code = $"{prefix}{(maxNum + 1):D4}" });
	}

	[HttpGet("{id:long}")]
	public async Task<IActionResult> Get(long id)
	{
		var entity = await _db.PurchaseReceipt
			.Include(p => p.Supplier)
			.Include(p => p.Company)
			.Include(p => p.Currency)
			.Include(p => p.SetWarehouse)
			.Include(p => p.StockTransactionStatus)
			.Include(p => p.Items).ThenInclude(i => i.Item)
			.Include(p => p.Items).ThenInclude(i => i.UOM)
			.Include(p => p.Taxes)
			.FirstOrDefaultAsync(p => p.Id == id);
		return entity is null ? NotFound() : Ok(entity);
	}

	[HttpPost]
	public async Task<IActionResult> Create(CreatePRDto dto)
	{
		var items = dto.Items.Select(i => new PurchaseReceiptItem
		{
			ItemId = i.ItemId,
			UOMId = i.UOMId,
			Quantity = i.Quantity,
			Rate = i.Rate,
			Amount = i.Quantity * i.Rate,
			InsertedBy = dto.InsertedBy,
			InsertedDate = DateTime.UtcNow,
		}).ToList();

		var taxes = dto.Taxes.Select(t => new PurchaseReceiptTax
		{
			Description = t.Description,
			Rate = t.Rate,
			Amount = t.Amount,
			InsertedBy = dto.InsertedBy,
			InsertedDate = DateTime.UtcNow,
		}).ToList();

		var entity = new PurchaseReceipt
		{
			InternalCode = dto.InternalCode,
			SupplierId = dto.SupplierId,
			CompanyId = dto.CompanyId,
			CurrencyId = dto.CurrencyId,
			SupplierDeliveryNote = dto.SupplierDeliveryNote,
			PostingDate = dto.PostingDate,
			PostingTime = dto.PostingTime,
			SetWarehouseId = dto.SetWarehouseId,
			StockTransactionStatusId = 1, // Draft
			Remarks = dto.Remarks,
			TotalQty = items.Sum(i => i.Quantity),
			Total = items.Sum(i => i.Amount),
			TaxTotal = taxes.Sum(t => t.Amount),
			InsertedBy = dto.InsertedBy,
			InsertedDate = DateTime.UtcNow,
			Items = items,
			Taxes = taxes,
		};
		entity.GrandTotal = entity.Total + entity.TaxTotal;

		_db.PurchaseReceipt.Add(entity);
		await _db.SaveChangesAsync();

		return Ok(entity);
	}

	[HttpPut("{id:long}")]
	public async Task<IActionResult> Update(long id, UpdatePRDto dto)
	{
		var entity = await _db.PurchaseReceipt.FindAsync(id);
		if (entity is null) return NotFound();

		entity.InternalCode = dto.InternalCode;
		entity.SupplierId = dto.SupplierId;
		entity.CompanyId = dto.CompanyId;
		entity.CurrencyId = dto.CurrencyId;
		entity.SupplierDeliveryNote = dto.SupplierDeliveryNote;
		entity.PostingDate = dto.PostingDate;
		entity.PostingTime = dto.PostingTime;
		entity.SetWarehouseId = dto.SetWarehouseId;
		entity.Remarks = dto.Remarks;
		await _db.SaveChangesAsync();

		return Ok(entity);
	}

	[HttpDelete("{id:long}")]
	public async Task<IActionResult> Delete(long id)
	{
		var entity = await _db.PurchaseReceipt.FindAsync(id);
		if (entity is null) return NotFound();
		_db.PurchaseReceipt.Remove(entity);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] List<long> ids)
	{
		var entities = await _db.PurchaseReceipt.Where(p => ids.Contains(p.Id)).ToListAsync();
		_db.PurchaseReceipt.RemoveRange(entities);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id:long}/submit")]
	public async Task<IActionResult> Submit(long id)
	{
		var entity = await _db.PurchaseReceipt.FindAsync(id);
		if (entity is null) return NotFound();
		entity.StockTransactionStatusId = 3; // Submitted
		await _db.SaveChangesAsync();
		return Ok(entity);
	}

	[HttpPatch("{id:long}/reissue")]
	public async Task<IActionResult> Reissue(long id)
	{
		var entity = await _db.PurchaseReceipt.FindAsync(id);
		if (entity is null) return NotFound();
		entity.StockTransactionStatusId = 1; // Draft
		await _db.SaveChangesAsync();
		return Ok(entity);
	}

	[HttpPatch("{id:long}/cancel")]
	public async Task<IActionResult> Cancel(long id)
	{
		var entity = await _db.PurchaseReceipt.FindAsync(id);
		if (entity is null) return NotFound();
		entity.StockTransactionStatusId = 4; // Cancelled
		await _db.SaveChangesAsync();
		return Ok(entity);
	}

	[HttpPatch("{id:long}/toggle-active")]
	public async Task<IActionResult> ToggleActive(long id)
	{
		var entity = await _db.PurchaseReceipt.FindAsync(id);
		if (entity is null) return NotFound();
		entity.IsActive = !entity.IsActive;
		await _db.SaveChangesAsync();
		return Ok(entity);
	}

	// ── Items sub-resource (edit-mode incremental add/remove) ──────────────────

	[HttpPost("{id:long}/items")]
	public async Task<IActionResult> AddItem(long id, CreatePRItemDto dto)
	{
		var exists = await _db.PurchaseReceipt.AnyAsync(p => p.Id == id);
		if (!exists) return NotFound();

		var item = new PurchaseReceiptItem
		{
			PurchaseReceiptId = id,
			ItemId = dto.ItemId,
			UOMId = dto.UOMId,
			Quantity = dto.Quantity,
			Rate = dto.Rate,
			Amount = dto.Quantity * dto.Rate,
			InsertedDate = DateTime.UtcNow,
		};
		_db.PurchaseReceiptItem.Add(item);
		await _db.SaveChangesAsync();

		await RecalculateTotalsAsync(id);

		return Ok(item);
	}

	[HttpDelete("{id:long}/items/{itemRowId:long}")]
	public async Task<IActionResult> RemoveItem(long id, long itemRowId)
	{
		var item = await _db.PurchaseReceiptItem
			.FirstOrDefaultAsync(i => i.Id == itemRowId && i.PurchaseReceiptId == id);
		if (item is null) return NotFound();

		_db.PurchaseReceiptItem.Remove(item);
		await _db.SaveChangesAsync();

		await RecalculateTotalsAsync(id);

		return NoContent();
	}

	// ── Taxes sub-resource (edit-mode incremental add/remove) ──────────────────

	[HttpPost("{id:long}/taxes")]
	public async Task<IActionResult> AddTax(long id, CreatePRTaxDto dto)
	{
		var exists = await _db.PurchaseReceipt.AnyAsync(p => p.Id == id);
		if (!exists) return NotFound();

		var tax = new PurchaseReceiptTax
		{
			PurchaseReceiptId = id,
			Description = dto.Description,
			Rate = dto.Rate,
			Amount = dto.Amount,
			InsertedDate = DateTime.UtcNow,
		};
		_db.PurchaseReceiptTax.Add(tax);
		await _db.SaveChangesAsync();

		await RecalculateTotalsAsync(id);

		return Ok(tax);
	}

	[HttpDelete("{id:long}/taxes/{taxRowId:long}")]
	public async Task<IActionResult> RemoveTax(long id, long taxRowId)
	{
		var tax = await _db.PurchaseReceiptTax
			.FirstOrDefaultAsync(t => t.Id == taxRowId && t.PurchaseReceiptId == id);
		if (tax is null) return NotFound();

		_db.PurchaseReceiptTax.Remove(tax);
		await _db.SaveChangesAsync();

		await RecalculateTotalsAsync(id);

		return NoContent();
	}
}
