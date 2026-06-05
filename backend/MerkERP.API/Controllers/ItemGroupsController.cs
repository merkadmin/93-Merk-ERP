using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemGroupsController(MerkDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await db.ItemGroup_cs.OrderBy(g => g.Name).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id) =>
        await db.ItemGroup_cs.FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Create(ItemGroup_cs e)
    {
        db.ItemGroup_cs.Add(e);
        await db.SaveChangesAsync();
        return Ok(e);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ItemGroup_cs e)
    {
        if (id != e.ItemGroupId) return BadRequest();
        db.ItemGroup_cs.Update(e);
        await db.SaveChangesAsync();
        return Ok(e);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await db.ItemGroup_cs.FindAsync(id);
        if (e is null) return NotFound();
        db.ItemGroup_cs.Remove(e);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
