namespace MerkERP.Core.Models;

public class WareHouse_cs
{
	public long WarehouseId { get; set; }
	public string Name { get; set; } = string.Empty;
	public long? ParentWarehouseId { get; set; }
	public bool IsGroup { get; set; }
	public bool IsActive { get; set; } = true;
	public bool IsFavorite { get; set; } = false;
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }

	public WareHouse_cs? ParentWarehouse { get; set; }
	public ICollection<WareHouse_cs> Children { get; set; } = [];
	public ICollection<WarehouseTransaction> Transactions { get; set; } = [];
	public ICollection<StockLedgerTransaction> StockLedgerTransactions { get; set; } = [];
}
