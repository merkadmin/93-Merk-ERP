using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockTransactionStatusesController : ControllerBase
{
    private readonly MerkDbContext _db;

    public StockTransactionStatusesController(MerkDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.StockTransactionStatus_s.OrderBy(t => t.Id).ToListAsync());
}
