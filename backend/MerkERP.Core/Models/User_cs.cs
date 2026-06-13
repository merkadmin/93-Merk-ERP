namespace MerkERP.Core.Models;

public class User_cs
{
	public long Id { get; set; }
	public string Name_EN { get; set; } = string.Empty;
	public string? Name_AR { get; set; }
	public string Login { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public long UserTypeId { get; set; }
	public UserType_s? UserType { get; set; }
	public string? Email { get; set; }
	public string? Description { get; set; }
}
