namespace MerkERP.Core.Models;

public class StockLedgerTransaction
{
    public int SLTId { get; set; }
    public int ItemId { get; set; }
    public int WarehouseId { get; set; }
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

    public Item_cs Item { get; set; } = null!;
    public WareHouse_cs Warehouse { get; set; } = null!;
}
