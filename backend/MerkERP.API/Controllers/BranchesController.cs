using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BranchesController : ControllerBase
{
	private readonly MerkDbContext _db;

	public BranchesController(MerkDbContext db)
	{
		_db = db;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await _db.Branch_cs.OrderBy(b => b.Name_EN).ToListAsync());

	[HttpGet("{id}")]
	public async Task<IActionResult> Get(long id) =>
		await _db.Branch_cs.FindAsync(id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(Branch_cs e)
	{
		_db.Branch_cs.Add(e);
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(long id, Branch_cs e)
	{
		if (id != e.Id) return BadRequest();
		_db.Branch_cs.Update(e);
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(long id)
	{
		var e = await _db.Branch_cs.FindAsync(id);
		if (e is null) return NotFound();
		_db.Branch_cs.Remove(e);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] long[] ids)
	{
		var entities = await _db.Branch_cs.Where(b => ids.Contains(b.Id)).ToListAsync();
		_db.Branch_cs.RemoveRange(entities);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id}/toggle-active")]
	public async Task<IActionResult> ToggleActive(long id)
	{
		var e = await _db.Branch_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsActive = !e.IsActive;
		await _db.SaveChangesAsync();
		return Ok(e);
	}
}
