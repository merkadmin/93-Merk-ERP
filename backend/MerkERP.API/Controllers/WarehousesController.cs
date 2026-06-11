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
		Ok(await db.WareHouse_cs.OrderBy(w => w.Name_EN).ToListAsync());

	[HttpGet("{id}")]
	public async Task<IActionResult> Get(long id) =>
		await db.WareHouse_cs.FindAsync(id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(WareHouse_cs e)
	{
		if (e.ParentWarehouseId == null)
		{
			bool rootExists = await db.WareHouse_cs.AnyAsync(w => w.ParentWarehouseId == null);
			if (rootExists)
				return BadRequest(new { message = "A root warehouse already exists. All new warehouses must have a parent." });
		}
		db.WareHouse_cs.Add(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(long id, WareHouse_cs e)
	{
		if (id != e.Id) return BadRequest();
		if (e.ParentWarehouseId == null)
		{
			bool otherRootExists = await db.WareHouse_cs.AnyAsync(w => w.ParentWarehouseId == null && w.Id != id);
			if (otherRootExists)
				return BadRequest(new { message = "A root warehouse already exists. Only one root warehouse is allowed." });
		}
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

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] long[] ids)
	{
		var entities = await db.WareHouse_cs.Where(w => ids.Contains(w.Id)).ToListAsync();
		db.WareHouse_cs.RemoveRange(entities);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id}/toggle-active")]
	public async Task<IActionResult> ToggleActive(long id)
	{
		var e = await db.WareHouse_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsActive = !e.IsActive;
		await db.SaveChangesAsync();
		return Ok(e);
	}
}
