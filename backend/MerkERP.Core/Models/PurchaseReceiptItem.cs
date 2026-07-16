namespace MerkERP.Core.Models;

public class PurchaseReceiptItem
{
	public long Id { get; set; }
	public long PurchaseReceiptId { get; set; }
	public long ItemId { get; set; }
	public long UOMId { get; set; }
	public decimal Quantity { get; set; }
	public decimal Rate { get; set; }
	public decimal Amount { get; set; }
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }

	public PurchaseReceipt? PurchaseReceipt { get; set; }
	public Item_cs? Item { get; set; }
	public UOM_cs? UOM { get; set; }
}
