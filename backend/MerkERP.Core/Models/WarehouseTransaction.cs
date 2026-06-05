namespace MerkERP.Core.Models;

public class WarehouseTransaction
{
	public long BinId { get; set; }
	public long ItemId { get; set; }
	public long WarehouseId { get; set; }
	public decimal ActualQty { get; set; }
	public decimal ReservedQty { get; set; }
	public decimal OrderedQty { get; set; }
	public decimal ValuationRate { get; set; }
	public bool IsActive { get; set; } = true;
	public bool IsFavorite { get; set; } = false;
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }

	public Item_cs Item { get; set; } = null!;
	public WareHouse_cs Warehouse { get; set; } = null!;
}
