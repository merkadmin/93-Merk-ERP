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

[ApiController]
[Route("api/[controller]")]
public class StockReconciliationTransactionsController : ControllerBase
{
    private readonly MerkDbContext _db;

    public StockReconciliationTransactionsController(MerkDbContext db)
    {
        _db = db;
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
        var entity = new StockReconciliationTransaction
        {
            StockTransactionTypeId   = dto.StockTransactionTypeId,
            StockTransactionStatusId = dto.StockTransactionStatusId,
            InternalCode             = dto.InternalCode,
            PostingDate              = dto.PostingDate,
            PostingTime              = dto.PostingTime,
            SetWarehouseId           = dto.SetWarehouseId,
            InsertedBy               = dto.InsertedBy,
            InsertedDate             = DateTime.UtcNow,
            Details = dto.Details.Select(d => new StockReconciliationTransactionDetail
            {
                ItemId       = d.ItemId,
                WarehouseId  = d.WarehouseId,
                Quantity     = d.Quantity,
                UOMId        = d.UOMId,
                InsertedBy   = dto.InsertedBy,
                InsertedDate = DateTime.UtcNow,
            }).ToList(),
        };
        _db.StockReconciliationTransaction.Add(entity);
        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateSRTDto dto)
    {
        var entity = await _db.StockReconciliationTransaction.FindAsync(id);
        if (entity is null) return NotFound();
        entity.StockTransactionTypeId   = dto.StockTransactionTypeId;
        entity.StockTransactionStatusId = dto.StockTransactionStatusId;
        entity.InternalCode             = dto.InternalCode;
        entity.PostingDate              = dto.PostingDate;
        entity.PostingTime              = dto.PostingTime;
        entity.SetWarehouseId           = dto.SetWarehouseId;
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
        var detail = new StockReconciliationTransactionDetail
        {
            StockReconciliationTransactionId = id,
            ItemId       = dto.ItemId,
            WarehouseId  = dto.WarehouseId,
            Quantity     = dto.Quantity,
            UOMId        = dto.UOMId,
            InsertedDate = DateTime.UtcNow,
        };
        _db.StockReconciliationTransactionDetail.Add(detail);
        await _db.SaveChangesAsync();
        return Ok(detail);
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
