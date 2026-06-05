using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UOMConversionGroupsController(MerkDbContext db) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await db.UOMConversionGroup_cs.OrderBy(g => g.Name_EN).ToListAsync());

	[HttpGet("{id}")]
	public async Task<IActionResult> Get(int id) =>
		await db.UOMConversionGroup_cs.FindAsync(id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(UOMConversionGroup_cs e)
	{
		db.UOMConversionGroup_cs.Add(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(int id, UOMConversionGroup_cs e)
	{
		if (id != e.Id) return BadRequest();
		db.UOMConversionGroup_cs.Update(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		var e = await db.UOMConversionGroup_cs.FindAsync(id);
		if (e is null) return NotFound();
		db.UOMConversionGroup_cs.Remove(e);
		await db.SaveChangesAsync();
		return NoContent();
	}
}
