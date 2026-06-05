using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UOMConversionFactorsController(MerkDbContext db) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await db.UOMConversionFactor_cs
			.Include(f => f.UOMFrom)
			.Include(f => f.UOMTo)
			.OrderBy(f => f.UOMFrom.Name)
			.ToListAsync());

	[HttpGet("{id}")]
	public async Task<IActionResult> Get(long id) =>
		await db.UOMConversionFactor_cs
			.Include(f => f.UOMFrom)
			.Include(f => f.UOMTo)
			.FirstOrDefaultAsync(f => f.Id == id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(UOMConversionFactor_cs e)
	{
		e.InsertedDate = DateTime.UtcNow;
		db.UOMConversionFactor_cs.Add(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(long id, UOMConversionFactor_cs e)
	{
		if (id != e.Id) return BadRequest();
		db.UOMConversionFactor_cs.Update(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(long id)
	{
		var e = await db.UOMConversionFactor_cs.FindAsync(id);
		if (e is null) return NotFound();
		db.UOMConversionFactor_cs.Remove(e);
		await db.SaveChangesAsync();
		return NoContent();
	}
}
