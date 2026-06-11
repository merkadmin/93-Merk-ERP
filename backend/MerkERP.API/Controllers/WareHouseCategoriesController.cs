using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WareHouseCategoriesController(MerkDbContext db) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await db.WareHouseCategory_cs.OrderBy(c => c.Name_EN).ToListAsync());

	[HttpGet("{id}")]
	public async Task<IActionResult> Get(long id) =>
		await db.WareHouseCategory_cs.FindAsync(id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(WareHouseCategory_cs e)
	{
		db.WareHouseCategory_cs.Add(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(long id, WareHouseCategory_cs e)
	{
		if (id != e.Id) return BadRequest();
		db.WareHouseCategory_cs.Update(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(long id)
	{
		var e = await db.WareHouseCategory_cs.FindAsync(id);
		if (e is null) return NotFound();
		db.WareHouseCategory_cs.Remove(e);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] long[] ids)
	{
		var entities = await db.WareHouseCategory_cs.Where(c => ids.Contains(c.Id)).ToListAsync();
		db.WareHouseCategory_cs.RemoveRange(entities);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id}/toggle-active")]
	public async Task<IActionResult> ToggleActive(long id)
	{
		var e = await db.WareHouseCategory_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsActive = !e.IsActive;
		await db.SaveChangesAsync();
		return Ok(e);
	}
}
