namespace MerkERP.Core.Models;

public class UOM_cs
{
	public long UOMId { get; set; }
	public string? InternalCode { get; set; }
	public string Name { get; set; } = string.Empty;
	public bool MustBeWholeNumber { get; set; }
	public bool IsActive { get; set; } = true;
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }
	public bool IsFavorite { get; set; } = false;

	public ICollection<Item_cs> DefaultForItems { get; set; } = [];
	public ICollection<ItemUOMConversion_cs> Conversions { get; set; } = [];
}
