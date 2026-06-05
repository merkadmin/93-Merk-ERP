using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController(MerkDbContext db) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await db.Item_cs
			.Include(i => i.ItemGroup)
			.Include(i => i.ItemType)
			.Include(i => i.DefaultUOM)
			.OrderBy(i => i.ItemCode)
			.ToListAsync());

	[HttpGet("{id}")]
	public async Task<IActionResult> Get(int id) =>
		await db.Item_cs
			.Include(i => i.ItemGroup)
			.Include(i => i.ItemType)
			.Include(i => i.DefaultUOM)
			.FirstOrDefaultAsync(i => i.ItemId == id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(Item_cs e)
	{
		db.Item_cs.Add(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(int id, Item_cs e)
	{
		if (id != e.ItemId) return BadRequest();
		db.Item_cs.Update(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		var e = await db.Item_cs.FindAsync(id);
		if (e is null) return NotFound();
		db.Item_cs.Remove(e);
		await db.SaveChangesAsync();
		return NoContent();
	}
}
