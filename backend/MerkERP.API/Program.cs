using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using MerkERP.API.Services;
using MerkERP.Core.Interfaces;
using MerkERP.DAL.Context;
using MerkERP.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MerkDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
     .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<StockService>();
builder.Services.AddScoped<TableRegistryService>();
builder.Services.AddSingleton<ExcelService>();

builder.Services.AddCors(o => o.AddPolicy("Angular", p =>
    p.WithOrigins("http://localhost:4200", "http://localhost:7710").AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddControllers().AddJsonOptions(o =>
    o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db       = scope.ServiceProvider.GetRequiredService<MerkDbContext>();
    var registry = scope.ServiceProvider.GetRequiredService<TableRegistryService>();
    db.Database.Migrate();
    await registry.SyncAsync();

    // One-time upgrade: hash any plain-text passwords still in the DB
    var users = await db.User_cs.ToListAsync();
    foreach (var u in users.Where(u => !u.Password.StartsWith("$2")))
        u.Password = BCrypt.Net.BCrypt.HashPassword(u.Password);
    await db.SaveChangesAsync();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("Angular");
app.MapControllers();
app.Run();
