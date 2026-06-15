namespace MerkERP.Core.Models;

public class StockReconciliationTransactionDate
{
	public long Id { get; set; }
	public long StockReconciliationTransactionId { get; set; }
	public DateTime ChangeDateTime { get; set; }
	public long StockTransactionStatusId { get; set; }
	public long StockTransactionTypeId { get; set; }
	public long? InsertedBy { get; set; }

	public StockReconciliationTransaction? StockReconciliationTransaction { get; set; }
	public StockTransactionStatus_s? StockTransactionStatus { get; set; }
	public StockTransactionType_s? StockTransactionType { get; set; }
}
