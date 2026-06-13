namespace MerkERP.Core.Models;

public class StockLedgerTransaction
{
	public long Id { get; set; }
	public long StockTransactionTypeId { get; set; }
	public string? InternalCode { get; set; }
	public long ItemId { get; set; }
	public long WareHouseId { get; set; }
	public decimal ActualQuantity { get; set; }
	public decimal QuantityAfterTransaction { get; set; }
	public decimal ValuationRate { get; set; }
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }

	public StockTransactionType_s? StockTransactionType { get; set; }
	public Item_cs? Item { get; set; }
	public WareHouse_cs? WareHouse { get; set; }
}
