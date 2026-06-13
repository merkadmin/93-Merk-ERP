namespace MerkERP.Core.Models;

public class StockReconciliationTransactionDetail
{
	public long Id { get; set; }
	public long StockReconciliationTransactionId { get; set; }
	public long ItemId { get; set; }
	public long WarehouseId { get; set; }
	public decimal Quantity { get; set; }
	public long UOMId { get; set; }
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }

	public StockReconciliationTransaction? StockReconciliationTransaction { get; set; }
	public Item_cs? Item { get; set; }
	public WareHouse_cs? Warehouse { get; set; }
	public UOM_cs? UOM { get; set; }
}
