namespace MerkERP.Core.Models;

public class UOMConversionFactor_cs
{
	public long Id { get; set; }
	public long UOMFromId { get; set; }
	public long UOMToId { get; set; }
	public double Value { get; set; }

	public UOM_cs UOMFrom { get; set; } = null!;
	public UOM_cs UOMTo { get; set; } = null!;
}
