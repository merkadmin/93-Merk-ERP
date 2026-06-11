namespace MerkERP.Core.Models;

public class Branch_cs
{
	public long Id { get; set; }
	public string Name_EN { get; set; } = string.Empty;
	public string? Name_AR { get; set; }
	public string? Description { get; set; }
	public bool IsActive { get; set; } = true;
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }
}
