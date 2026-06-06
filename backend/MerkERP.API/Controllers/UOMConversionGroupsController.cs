using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

public record BulkActiveDto(List<int> Ids, bool IsActive);

[ApiController]
[Route("api/[controller]")]
public class UOMConversionGroupsController(MerkDbContext db) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAll() =>
		Ok(await db.UOMConversionGroup_cs.ToListAsync());

	[HttpGet("nextcode")]
	public async Task<IActionResult> NextCode()
	{
		const string prefix = "UGR-";
		var codes = await db.UOMConversionGroup_cs
			.Where(g => g.InternalCode != null && g.InternalCode.StartsWith(prefix))
			.Select(g => g.InternalCode!)
			.ToListAsync();

		var maxNum = codes
			.Select(c => int.TryParse(c[prefix.Length..], out var n) ? n : 0)
			.DefaultIfEmpty(0)
			.Max();

		return Ok(new { code = $"{prefix}{(maxNum + 1):D3}" });
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> Get(int id) =>
		await db.UOMConversionGroup_cs.FindAsync(id) is { } e ? Ok(e) : NotFound();

	[HttpPost]
	public async Task<IActionResult> Create(UOMConversionGroup_cs e)
	{
		db.UOMConversionGroup_cs.Add(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Update(int id, UOMConversionGroup_cs e)
	{
		if (id != e.Id) return BadRequest();
		db.UOMConversionGroup_cs.Update(e);
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
	{
		var e = await db.UOMConversionGroup_cs.FindAsync(id);
		if (e is null) return NotFound();
		db.UOMConversionGroup_cs.Remove(e);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpPatch("{id:int}/toggle-favorite")]
	public async Task<IActionResult> ToggleFavorite(int id)
	{
		var e = await db.UOMConversionGroup_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsFavorite = !e.IsFavorite;
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("{id:int}/toggle-active")]
	public async Task<IActionResult> ToggleActive(int id)
	{
		var e = await db.UOMConversionGroup_cs.FindAsync(id);
		if (e is null) return NotFound();
		e.IsActive = !e.IsActive;
		await db.SaveChangesAsync();
		return Ok(e);
	}

	[HttpPatch("bulk-active")]
	public async Task<IActionResult> BulkSetActive([FromBody] BulkActiveDto dto)
	{
		var entities = await db.UOMConversionGroup_cs
			.Where(g => dto.Ids.Contains(g.Id))
			.ToListAsync();
		entities.ForEach(e => e.IsActive = dto.IsActive);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("bulk")]
	public async Task<IActionResult> DeleteBulk([FromBody] List<int> ids)
	{
		var entities = await db.UOMConversionGroup_cs
			.Where(g => ids.Contains(g.Id))
			.ToListAsync();
		db.UOMConversionGroup_cs.RemoveRange(entities);
		await db.SaveChangesAsync();
		return NoContent();
	}
}
