using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.Core.DTOs;
using MerkERP.DAL.Context;
using MerkERP.DAL.Repositories;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController(MerkDbContext db, StockService stockService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCurrentStock() =>
        Ok(await db.WarehouseTransaction
            .Include(b => b.Item)
            .Include(b => b.Warehouse)
            .OrderBy(b => b.Item.ItemCode)
            .ToListAsync());

    [HttpGet("ledger")]
    public async Task<IActionResult> GetLedger(
        [FromQuery] int? itemId,
        [FromQuery] int? warehouseId) =>
        Ok(await db.StockLedgerTransaction
            .Include(s => s.Item)
            .Include(s => s.Warehouse)
            .Where(s => (itemId == null || s.ItemId == itemId)
                     && (warehouseId == null || s.WarehouseId == warehouseId))
            .OrderByDescending(s => s.PostingDate)
            .ThenByDescending(s => s.PostingTime)
            .ToListAsync());

    [HttpPost("movement")]
    public async Task<IActionResult> PostMovement(StockMovementDto dto)
    {
        if (dto.Qty == 0) return BadRequest("Qty cannot be zero.");
        var result = await stockService.PostMovementAsync(dto);
        return Ok(result);
    }
}
