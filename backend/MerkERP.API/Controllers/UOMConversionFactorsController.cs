using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

public record BulkFactorActiveDto(List<long> Ids, bool IsActive);

[ApiController]
[Route("api/[controller]")]
public class UOMConversionFactorsController(MerkDbContext db) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await db.UOMConversionFactor_cs
			.Include(f => f.UOMFrom)
			.Include(f => f.UOMTo)
			.Include(f => f.UOMConversionGroup)
			.OrderBy(f => f.UOMFrom.Name_EN)
			.ToListAsync());

	[HttpGet("nextcode")]
	public async Task<IActionResult> NextCode()
	{
		const string prefix = "UCF-";
		var codes = await db.UOMConversionFactor_cs
			.Where(f => f.InternalCode != null && f.InternalCode.StartsWith(prefix))
			.Select(f => f.InternalCode!)
			.ToListAsync();

		var maxNum = codes
			.Select(c => int.TryParse(c[prefix.Length..], out var n) ? n : 0)
			.DefaultIfEmpty(0)
			.Max();

		return Ok(new { code = $"{prefix}{(maxNum + 1):D3}" });
	}

	[HttpGet("{id:long}")]
	public async Task<IActionResult> Get(long id) =>
		await db.UOMConversionFactor_cs
			.Include(f => f.UOMFrom)
			.Include(f => f.UOMTo)
			.Include(f => f.UOMConversionGroup)
			.FirstOrDefaultAsync(f => f.Id == id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(UOMConversionFactor_cs e)
	{
		e.UOMFrom            = null;
		e.UOMTo              = null;
		e.UOMConversionGroup = null;
		e.InsertedDate       = DateTime.UtcNow;
		db.UOMConversionFactor_cs.Add(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id:long}")]
	public async Task<IActionResult> Update(long id, UOMConversionFactor_cs e)
	{
		if (id != e.Id) return BadRequest();
		var existing = await db.UOMConversionFactor_cs.FindAsync(id);
		if (existing is null) return NotFound();
		existing.UOMFromId             = e.UOMFromId;
		existing.UOMToId               = e.UOMToId;
		existing.Value                 = e.Value;
		existing.UOMConversionGroupId  = e.UOMConversionGroupId;
		existing.InternalCode          = e.InternalCode;
		existing.IsActive              = e.IsActive;
		existing.IsFavorite            = e.IsFavorite;
		await db.SaveChangesAsync();
		return Ok(existing);
	}

	[HttpDelete("{id:long}")]
	public async Task<IActionResult> Delete(long id)
	{
		var e = await db.UOMConversionFactor_cs.FindAsync(id);
		if (e is null) return NotFound();
		db.UOMConversionFactor_cs.Remove(e);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id:long}/toggle-favorite")]
	public async Task<IActionResult> ToggleFavorite(long id)
	{
		var e = await db.UOMConversionFactor_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsFavorite = !e.IsFavorite;
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("{id:long}/toggle-active")]
	public async Task<IActionResult> ToggleActive(long id)
	{
		var e = await db.UOMConversionFactor_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsActive = !e.IsActive;
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("bulk-active")]
	public async Task<IActionResult> BulkSetActive([FromBody] BulkFactorActiveDto dto)
	{
		var entities = await db.UOMConversionFactor_cs
			.Where(f => dto.Ids.Contains(f.Id))
			.ToListAsync();
		entities.ForEach(e => e.IsActive = dto.IsActive);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] List<long> ids)
	{
		var entities = await db.UOMConversionFactor_cs
			.Where(f => ids.Contains(f.Id))
			.ToListAsync();
		db.UOMConversionFactor_cs.RemoveRange(entities);
		await db.SaveChangesAsync();
		return NoContent();
	}
}
