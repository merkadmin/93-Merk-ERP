using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

public record BulkUomActiveDto(List<long> Ids, bool IsActive);

[ApiController]
[Route("api/[controller]")]
public class UOMsController(MerkDbContext db) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await db.UOM_cs.ToListAsync());

	[HttpGet("nextcode")]
	public async Task<IActionResult> NextCode()
	{
		const string prefix = "UOM-";
		var codes = await db.UOM_cs
			.Where(u => u.InternalCode != null && u.InternalCode.StartsWith(prefix))
			.Select(u => u.InternalCode!)
			.ToListAsync();

		var maxNum = codes
			.Select(c => int.TryParse(c[prefix.Length..], out var n) ? n : 0)
			.DefaultIfEmpty(0)
			.Max();

		return Ok(new { code = $"{prefix}{(maxNum + 1):D3}" });
	}

	[HttpGet("{id:long}")]
	public async Task<IActionResult> Get(long id) =>
		await db.UOM_cs.FindAsync(id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(UOM_cs e)
	{
		e.DefaultForItems = [];
		e.Conversions     = [];
		db.UOM_cs.Add(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id:long}")]
	public async Task<IActionResult> Update(long id, UOM_cs e)
	{
		if (id != e.Id) return BadRequest();
		var existing = await db.UOM_cs.FindAsync(id);
		if (existing is null) return NotFound();
		existing.InternalCode      = e.InternalCode;
		existing.Name_EN           = e.Name_EN;
		existing.Name_AR           = e.Name_AR;
		existing.MustBeWholeNumber = e.MustBeWholeNumber;
		existing.IsActive          = e.IsActive;
		existing.IsFavorite        = e.IsFavorite;
		await db.SaveChangesAsync();
		return Ok(existing);
	}

	[HttpDelete("{id:long}")]
	public async Task<IActionResult> Delete(long id)
	{
		var e = await db.UOM_cs.FindAsync(id);
		if (e is null) return NotFound();
		db.UOM_cs.Remove(e);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id:long}/toggle-favorite")]
	public async Task<IActionResult> ToggleFavorite(long id)
	{
		var e = await db.UOM_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsFavorite = !e.IsFavorite;
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("{id:long}/toggle-active")]
	public async Task<IActionResult> ToggleActive(long id)
	{
		var e = await db.UOM_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsActive = !e.IsActive;
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("bulk-active")]
	public async Task<IActionResult> BulkSetActive([FromBody] BulkUomActiveDto dto)
	{
		var entities = await db.UOM_cs
			.Where(u => dto.Ids.Contains(u.Id))
			.ToListAsync();
		entities.ForEach(e => e.IsActive = dto.IsActive);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] List<long> ids)
	{
		var entities = await db.UOM_cs
			.Where(u => ids.Contains(u.Id))
			.ToListAsync();
		db.UOM_cs.RemoveRange(entities);
		await db.SaveChangesAsync();
		return NoContent();
	}
}
