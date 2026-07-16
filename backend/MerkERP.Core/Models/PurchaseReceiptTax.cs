namespace MerkERP.Core.Models;

public class PurchaseReceiptTax
{
	public long Id { get; set; }
	public long PurchaseReceiptId { get; set; }
	public string Description { get; set; } = string.Empty;
	public decimal? Rate { get; set; }
	public decimal Amount { get; set; }
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }

	public PurchaseReceipt? PurchaseReceipt { get; set; }
}
