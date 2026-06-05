namespace MerkERP.Core.Models;

public class StockLedgerTransaction
{
	public long SLTId { get; set; }
	public long ItemId { get; set; }
	public long WarehouseId { get; set; }
	public DateOnly PostingDate { get; set; }
	public TimeOnly PostingTime { get; set; }
	public decimal ActualQty { get; set; }
	public decimal QtyAfterTransaction { get; set; }
	public decimal ValuationRate { get; set; }
	public decimal StockValue { get; set; }
	public string VoucherType { get; set; } = string.Empty;
	public string VoucherNo { get; set; } = string.Empty;
	public string? BatchNo { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public bool IsActive { get; set; } = true;
	public bool IsFavorite { get; set; } = false;
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }

	public Item_cs Item { get; set; } = null!;
	public WareHouse_cs Warehouse { get; set; } = null!;
}
