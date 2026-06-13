using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Models;
using MerkERP.Core.Models.Enums;
using MerkERP.DAL.Context;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly MerkDbContext _db;

    public AuthController(MerkDbContext db)
    {
        _db = db;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Login) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { message = "Login and password are required." });

        var user = await _db.User_cs
            .Include(u => u.UserType)
            .FirstOrDefaultAsync(u => u.Login == req.Login);

        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.Password))
            return Unauthorized(new { message = "Invalid login or password." });

        return Ok(BuildUserResponse(user));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name_EN) || string.IsNullOrWhiteSpace(req.Login) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { message = "Name, login and password are required." });

        if (await _db.User_cs.AnyAsync(u => u.Login == req.Login))
            return Conflict(new { message = "This login is already taken." });

        var user = new User_cs
        {
            Name_EN     = req.Name_EN,
            Name_AR     = req.Name_AR,
            Login       = req.Login,
            Password    = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Email       = req.Email,
            UserTypeId  = (long)UserTypeEnum.RegularUser,
        };

        _db.User_cs.Add(user);
        await _db.SaveChangesAsync();

        await _db.Entry(user).Reference(u => u.UserType).LoadAsync();

        return Ok(BuildUserResponse(user));
    }

    private static object BuildUserResponse(User_cs user) => new
    {
        user.Id,
        user.Name_EN,
        user.Name_AR,
        user.Login,
        user.Email,
        user.UserTypeId,
        userTypeName_EN = user.UserType?.Name_EN,
        userTypeName_AR = user.UserType?.Name_AR,
    };
}

public record LoginRequest(string Login, string Password);
public record RegisterRequest(string Name_EN, string? Name_AR, string Login, string Password, string? Email);
