namespace MerkERP.Core.Models;

public class Item_cs
{
	public long ItemId { get; set; }
	public string ItemCode { get; set; } = string.Empty;
	public string ItemName { get; set; } = string.Empty;
	public long ItemGroupId { get; set; }
	public long ItemTypeId { get; set; }
	public long DefaultUOMId { get; set; }
	public string? Description { get; set; }
	public bool HasBatch { get; set; }
	public bool HasSerial { get; set; }
	public bool IsActive { get; set; } = true;

	public ItemGroup_cs ItemGroup { get; set; } = null!;
	public ItemType_s ItemType { get; set; } = null!;
	public UOM_cs DefaultUOM { get; set; } = null!;
	public ICollection<ItemUOMConversion_cs> UOMConversions { get; set; } = [];
	public ICollection<WarehouseTransaction> WarehouseTransactions { get; set; } = [];
	public ICollection<StockLedgerTransaction> StockLedgerTransactions { get; set; } = [];
}
