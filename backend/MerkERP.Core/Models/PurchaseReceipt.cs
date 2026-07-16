namespace MerkERP.Core.Models;

public class PurchaseReceipt
{
	public long Id { get; set; }
	public string? InternalCode { get; set; }
	public long SupplierId { get; set; }
	public long CompanyId { get; set; }
	public long CurrencyId { get; set; }
	public string? SupplierDeliveryNote { get; set; }
	public DateOnly PostingDate { get; set; }
	public TimeOnly PostingTime { get; set; }
	public long? SetWarehouseId { get; set; }
	public long StockTransactionStatusId { get; set; } = 1;
	public string? Remarks { get; set; }
	public decimal TotalQty { get; set; }
	public decimal Total { get; set; }
	public decimal TaxTotal { get; set; }
	public decimal GrandTotal { get; set; }
	public bool IsActive { get; set; } = true;
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }

	public Supplier_cs? Supplier { get; set; }
	public Company_cs? Company { get; set; }
	public Currency_s? Currency { get; set; }
	public WareHouse_cs? SetWarehouse { get; set; }
	public StockTransactionStatus_s? StockTransactionStatus { get; set; }
	public ICollection<PurchaseReceiptItem> Items { get; set; } = [];
	public ICollection<PurchaseReceiptTax> Taxes { get; set; } = [];
}
