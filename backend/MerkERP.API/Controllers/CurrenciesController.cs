using Microsoft.AspNetCore.Mvc;
using MerkERP.Core.Interfaces;
using MerkERP.Core.Models;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrenciesController(IRepository<Currency_s> repo) : ControllerBase
{
	[HttpGet] public async Task<IActionResult> GetAll() => Ok(await repo.GetAllAsync());
	[HttpGet("{id}")] public async Task<IActionResult> Get(long id) => await repo.GetByIdAsync(id) is { } e ? Ok(e) : NotFound();
	[HttpPost] public async Task<IActionResult> Create(Currency_s e) => Ok(await repo.CreateAsync(e));
	[HttpPut("{id}")]
	public async Task<IActionResult> Update(long id, Currency_s e)
	{
		if (id != e.Id) return BadRequest();
		return Ok(await repo.UpdateAsync(e));
	}
	[HttpDelete("{id}")] public async Task<IActionResult> Delete(long id) => await repo.DeleteAsync(id) ? NoContent() : NotFound();
}
