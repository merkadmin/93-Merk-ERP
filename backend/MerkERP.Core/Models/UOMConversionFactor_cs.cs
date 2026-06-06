namespace MerkERP.Core.Models;

public class UOMConversionFactor_cs
{
	public long Id { get; set; }
	public long UOMFromId { get; set; }
	public long UOMToId { get; set; }
	public double Value { get; set; }
	public bool IsActive { get; set; } = true;
	public bool IsFavorite { get; set; } = false;
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }

	public UOM_cs? UOMFrom { get; set; }
	public UOM_cs? UOMTo { get; set; }
}
