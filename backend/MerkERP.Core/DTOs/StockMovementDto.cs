namespace MerkERP.Core.DTOs;

public class StockMovementDto
{
    public int ItemId { get; set; }
    public int WarehouseId { get; set; }
    public decimal Qty { get; set; }
    public decimal ValuationRate { get; set; }
    public string VoucherType { get; set; } = "StockEntry";
    public string VoucherNo { get; set; } = string.Empty;
    public string? BatchNo { get; set; }
    public DateOnly PostingDate { get; set; }
}
