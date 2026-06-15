namespace MerkERP.Core.Models;

public class StockReconciliationTransaction
{
	public long Id { get; set; }
	public long StockTransactionTypeId { get; set; }
	public long StockTransactionStatusId { get; set; }
	public string? InternalCode { get; set; }
	public DateOnly PostingDate { get; set; }
	public TimeOnly PostingTime { get; set; }
	public long? SetWarehouseId { get; set; }
	public bool IsActive { get; set; } = true;
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }

	public StockTransactionType_s? StockTransactionType { get; set; }
	public StockTransactionStatus_s? StockTransactionStatus { get; set; }
	public WareHouse_cs? SetWarehouse { get; set; }
	public ICollection<StockReconciliationTransactionDetail> Details { get; set; } = [];
}
