using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BarcodeTypesController : ControllerBase
{
	private readonly MerkDbContext _db;

	public BarcodeTypesController(MerkDbContext db)
	{
		_db = db;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await _db.BarcodeType_s.OrderBy(b => b.BarcodeTypeId).ToListAsync());
}
