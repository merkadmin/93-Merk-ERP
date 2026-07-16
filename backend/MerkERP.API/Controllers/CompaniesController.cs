using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
	private readonly MerkDbContext _db;

	public CompaniesController(MerkDbContext db)
	{
		_db = db;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await _db.Company_cs
			.Include(c => c.DefaultCurrency)
			.OrderBy(c => c.Name_EN)
			.ToListAsync());

	[HttpGet("nextcode")]
	public async Task<IActionResult> NextCode()
	{
		const string prefix = "CO-";
		var codes = await _db.Company_cs
			.Where(c => c.InternalCode != null && c.InternalCode.StartsWith(prefix))
			.Select(c => c.InternalCode!)
			.ToListAsync();

		var maxNum = codes
			.Select(c => int.TryParse(c[prefix.Length..], out var n) ? n : 0)
			.DefaultIfEmpty(0)
			.Max();

		return Ok(new { code = $"{prefix}{(maxNum + 1):D3}" });
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> Get(long id) =>
		await _db.Company_cs
			.Include(c => c.DefaultCurrency)
			.FirstOrDefaultAsync(c => c.Id == id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(Company_cs e)
	{
		_db.Company_cs.Add(e);
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(long id, Company_cs e)
	{
		if (id != e.Id) return BadRequest();
		_db.Company_cs.Update(e);
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(long id)
	{
		var e = await _db.Company_cs.FindAsync(id);
		if (e is null) return NotFound();
		_db.Company_cs.Remove(e);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] long[] ids)
	{
		var entities = await _db.Company_cs.Where(c => ids.Contains(c.Id)).ToListAsync();
		_db.Company_cs.RemoveRange(entities);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id}/toggle-active")]
	public async Task<IActionResult> ToggleActive(long id)
	{
		var e = await _db.Company_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsActive = !e.IsActive;
		await _db.SaveChangesAsync();
		return Ok(e);
	}
}
