namespace MerkERP.Core.Models;

public class ItemUOMConversion_cs
{
	public int Id { get; set; }
	public int ItemId { get; set; }
	public int UOMId { get; set; }
	public decimal ConversionFactor { get; set; }

	public Item_cs Item { get; set; } = null!;
	public UOM_cs UOM { get; set; } = null!;
}
