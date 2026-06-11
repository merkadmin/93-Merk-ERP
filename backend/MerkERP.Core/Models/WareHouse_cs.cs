namespace MerkERP.Core.Models;

public class WareHouse_cs
{
	public long Id { get; set; }
	public string? InternalCode { get; set; }
	public string Name_EN { get; set; } = string.Empty;
	public string? Name_AR { get; set; }
	public string? Description { get; set; }
	public long? ParentWarehouseId { get; set; }
	public long? WareHouseTypeId { get; set; }
	public long? WareHouseCategoryId { get; set; }
	public bool IsParent { get; set; }
	public bool IsActive { get; set; } = true;
	public bool IsFavorite { get; set; } = false;
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }

	public WareHouse_cs? ParentWarehouse { get; set; }
	public WareHouseType_s? WareHouseType { get; set; }
	public WareHouseCategory_cs? WareHouseCategory { get; set; }
	public ICollection<WareHouse_cs> Children { get; set; } = [];
	public ICollection<WarehouseTransaction> Transactions { get; set; } = [];
	public ICollection<StockLedgerTransaction> StockLedgerTransactions { get; set; } = [];
}
