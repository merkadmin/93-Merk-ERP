namespace MerkERP.Core.Models;

public class Item_UOM_Barcode_cs
{
	public long Id { get; set; }
	public long ItemId { get; set; }
	public long BarcodeTypeId { get; set; }
	public long UOMId { get; set; }
	public string Barcode { get; set; } = string.Empty;

	public Item_cs? Item { get; set; }
	public BarcodeType_s? BarcodeType { get; set; }
	public UOM_cs? UOM { get; set; }
}
