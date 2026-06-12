using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MerkERP.DAL.Context;

public class MerkDbContextFactory : IDesignTimeDbContextFactory<MerkDbContext>
{
    public MerkDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<MerkDbContext>()
            .UseSqlServer("Server=RAYADDELLWS\\RAYADDELLSQLSRV;Database=MerkERPDB;Trusted_Connection=True;TrustServerCertificate=True;")
            .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
            .Options;

        return new MerkDbContext(options);
    }
}
