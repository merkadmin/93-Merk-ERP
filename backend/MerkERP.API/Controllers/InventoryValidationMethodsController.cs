using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryValidationMethodsController : ControllerBase
{
	private readonly MerkDbContext _db;

	public InventoryValidationMethodsController(MerkDbContext db)
	{
		_db = db;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await _db.InventoryValidationMethod_s.OrderBy(m => m.InventoryValidationMethodId).ToListAsync());
}
