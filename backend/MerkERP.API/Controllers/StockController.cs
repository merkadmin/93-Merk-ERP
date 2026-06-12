using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.Core.DTOs;
using MerkERP.DAL.Context;
using MerkERP.DAL.Repositories;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
	private readonly MerkDbContext _db;
	private readonly StockService  _stockService;

	public StockController(MerkDbContext db, StockService stockService)
	{
		_db           = db;
		_stockService = stockService;
	}

	[HttpGet]
	public async Task<IActionResult> GetCurrentStock() =>
		Ok(await _db.WarehouseTransaction
			.Include(b => b.Item)
			.Include(b => b.Warehouse)
			.OrderBy(b => b.Item.InternalCode)
			.ToListAsync());

	[HttpGet("ledger")]
	public async Task<IActionResult> GetLedger(
		[FromQuery] long? itemId,
		[FromQuery] long? warehouseId) =>
		Ok(await _db.StockLedgerTransaction
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
		var result = await _stockService.PostMovementAsync(dto);
		return Ok(result);
	}
}
