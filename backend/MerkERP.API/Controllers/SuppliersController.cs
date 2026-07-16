using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
	private readonly MerkDbContext _db;

	public SuppliersController(MerkDbContext db)
	{
		_db = db;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await _db.Supplier_cs
			.Include(s => s.SupplierType)
			.Include(s => s.DefaultCurrency)
			.OrderBy(s => s.Name_EN)
			.ToListAsync());

	[HttpGet("nextcode")]
	public async Task<IActionResult> NextCode()
	{
		const string prefix = "SUP-";
		var codes = await _db.Supplier_cs
			.Where(s => s.InternalCode != null && s.InternalCode.StartsWith(prefix))
			.Select(s => s.InternalCode!)
			.ToListAsync();

		var maxNum = codes
			.Select(c => int.TryParse(c[prefix.Length..], out var n) ? n : 0)
			.DefaultIfEmpty(0)
			.Max();

		return Ok(new { code = $"{prefix}{(maxNum + 1):D4}" });
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> Get(long id) =>
		await _db.Supplier_cs
			.Include(s => s.SupplierType)
			.Include(s => s.DefaultCurrency)
			.FirstOrDefaultAsync(s => s.Id == id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(Supplier_cs e)
	{
		_db.Supplier_cs.Add(e);
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(long id, Supplier_cs e)
	{
		if (id != e.Id) return BadRequest();
		_db.Supplier_cs.Update(e);
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(long id)
	{
		var e = await _db.Supplier_cs.FindAsync(id);
		if (e is null) return NotFound();
		_db.Supplier_cs.Remove(e);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] long[] ids)
	{
		var entities = await _db.Supplier_cs.Where(s => ids.Contains(s.Id)).ToListAsync();
		_db.Supplier_cs.RemoveRange(entities);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id}/toggle-active")]
	public async Task<IActionResult> ToggleActive(long id)
	{
		var e = await _db.Supplier_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsActive = !e.IsActive;
		await _db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("{id}/toggle-hold")]
	public async Task<IActionResult> ToggleHold(long id)
	{
		var e = await _db.Supplier_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsOnHold = !e.IsOnHold;
		await _db.SaveChangesAsync();
		return Ok(e);
	}
}
