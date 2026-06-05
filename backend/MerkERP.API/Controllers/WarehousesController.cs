using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehousesController(MerkDbContext db) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await db.WareHouse_cs.OrderBy(w => w.Name).ToListAsync());

	[HttpGet("{id}")]
	public async Task<IActionResult> Get(long id) =>
		await db.WareHouse_cs.FindAsync(id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(WareHouse_cs e)
	{
		db.WareHouse_cs.Add(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(long id, WareHouse_cs e)
	{
		if (id != e.WarehouseId) return BadRequest();
		db.WareHouse_cs.Update(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(long id)
	{
		var e = await db.WareHouse_cs.FindAsync(id);
		if (e is null) return NotFound();
		db.WareHouse_cs.Remove(e);
		await db.SaveChangesAsync();
		return NoContent();
	}
}
