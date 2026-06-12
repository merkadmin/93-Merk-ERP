using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.API.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetadataController(MerkDbContext db) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAll()
	{
		var dict = await BuildAllAsync();
		return Ok(dict);
	}

	[HttpGet("{entity}")]
	public async Task<IActionResult> Get(string entity)
	{
		var tableNameId = await db.TableName_s
			.Where(t => t.EntityKey != null && t.EntityKey.ToLower() == entity.ToLower())
			.Select(t => t.Id)
			.FirstOrDefaultAsync();

		if (tableNameId == 0)
			return NotFound(new { message = $"No metadata registered for '{entity}'." });

		var rows = await db.Database
			.SqlQuery<ColumnMetaRow>($"EXEC sp_GetTableMetaData @TableNameId = {tableNameId}")
			.ToListAsync();

		if (rows.Count == 0)
			return NotFound(new { message = $"No metadata registered for '{entity}'." });

		var first = rows.First();
		var meta = new EntityMeta(
			first.EntityKey, first.TableName, first.TableName,
			rows.Select(r => new ColumnMeta(
				r.Key, r.LabelEN, r.LabelAR, r.ColumnOrder, r.EntityProperty,
				r.ForeignKeyProperty, r.FilterType, r.DataType, r.RenderAs,
				r.IsSortable, r.IsFilterable, r.IsVisible, r.MinWidth))
			.ToArray());
		return Ok(meta);
	}

	// ── helpers ──────────────────────────────────────────────────────────────

	private async Task<Dictionary<string, EntityMeta>> BuildAllAsync()
	{
		var rows = await db.TableMetaData
			.Include(t => t.TableName)
			.Where(t => t.TableName != null && t.TableName.EntityKey != null)
			.OrderBy(t => t.TableName!.EntityKey)
			.ThenBy(t => t.ColumnOrder)
			.ToListAsync();

		return rows
			.GroupBy(r => r.TableName!.EntityKey!)
			.ToDictionary(
				g => g.Key,
				g =>
				{
					var tn = g.First().TableName!;
					return new EntityMeta(g.Key, tn.Name, tn.Name, g.Select(ToColumnMeta).ToArray());
				});
	}

	private static ColumnMeta ToColumnMeta(MerkERP.Core.Models.TableMetaData r) =>
		new(r.Key, r.LabelEN, r.LabelAR, r.ColumnOrder, r.EntityProperty,
			r.ForeignKeyProperty, r.FilterType, r.DataType, r.RenderAs,
			r.IsSortable, r.IsFilterable, r.IsVisible, r.MinWidth);
}
