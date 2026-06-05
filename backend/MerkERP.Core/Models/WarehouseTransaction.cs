namespace MerkERP.Core.Models;

public class WarehouseTransaction
{
	public int BinId { get; set; }
	public int ItemId { get; set; }
	public int WarehouseId { get; set; }
	public decimal ActualQty { get; set; }
	public decimal ReservedQty { get; set; }
	public decimal OrderedQty { get; set; }
	public decimal ValuationRate { get; set; }

	public Item_cs Item { get; set; } = null!;
	public WareHouse_cs Warehouse { get; set; } = null!;
}
