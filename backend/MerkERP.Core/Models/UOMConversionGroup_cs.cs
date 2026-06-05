namespace MerkERP.Core.Models;

public class UOMConversionGroup_cs
{
	public int Id { get; set; }
	public string Name_EN { get; set; } = string.Empty;
	public string Name_AR { get; set; } = string.Empty;
	public bool IsActive { get; set; } = true;
	public bool IsFavorite { get; set; } = false;
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }
}
