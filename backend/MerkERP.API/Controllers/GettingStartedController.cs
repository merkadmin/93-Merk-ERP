using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GettingStartedController : ControllerBase
{
    private readonly MerkDbContext _db;

    public GettingStartedController(MerkDbContext db) => _db = db;

    [HttpGet("status")]
    public async Task<IActionResult> Status()
    {
        var warehouse  = await _db.WareHouse_cs.AnyAsync();
        var item       = await _db.Item_cs.AnyAsync();
        // Only Opening Balance entries (typeId = 1) count for the getting-started step
        var stockRecon = await _db.StockReconciliationTransaction.AnyAsync(t => t.StockTransactionTypeId == 1);

        return Ok(new { warehouse, item, stockRecon });
    }
}
