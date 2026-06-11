using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;
using MerkERP.DAL.Context;

namespace MerkERP.API.Services;

/// <summary>
/// Runs once at startup and ensures every physical table in the EF model
/// has a corresponding row in <see cref="TableName_s"/>.
/// New tables discovered on startup receive the next available integer Id.
/// Existing rows are never modified or deleted so Ids remain stable.
/// </summary>
public class TableRegistryService(MerkDbContext db)
{
    public async Task SyncAsync()
    {
        // Collect every table name that EF Core maps to a physical SQL table.
        var efTables = db.Model
            .GetEntityTypes()
            .Select(e => e.GetTableName())
            .Where(t => !string.IsNullOrEmpty(t))
            .Distinct()
            .OrderBy(t => t)
            .ToList();

        // Load currently registered names.
        var existing = await db.TableName_s
            .ToDictionaryAsync(t => t.Name, t => t.Id);

        // Determine the next available Id (start at 1 if the table is empty).
        int nextId = existing.Count > 0 ? existing.Values.Max() + 1 : 1;

        var toAdd = new List<TableName_s>();
        foreach (var name in efTables)
        {
            if (!existing.ContainsKey(name!))
            {
                toAdd.Add(new TableName_s { Id = nextId++, Name = name! });
            }
        }

        if (toAdd.Count > 0)
        {
            db.TableName_s.AddRange(toAdd);
            await db.SaveChangesAsync();
        }
    }
}
