namespace MerkERP.Core.Models;

public class Item_cs
{
	public long Id { get; set; }
	public string InternalCode { get; set; } = string.Empty;
	public string Name_EN { get; set; } = string.Empty;
	public string? Name_AR { get; set; }
	public long ItemGroupId { get; set; }
	public long ItemTypeId { get; set; }
	public long DefaultUOMId { get; set; }
	public long? DefaultPurchaseUOMId { get; set; }
	public bool AcceptSelling { get; set; } = true;
	public long? DefaultSellingUOMId { get; set; }
	public string? Description { get; set; }
	public long? OpeningStock { get; set; }
	public DateTime? ExpirationDate { get; set; }
	public int? MinOrderQuantity { get; set; }
	public long? SafetyStock { get; set; }
	public bool IsActive { get; set; } = true;
	public bool IsFavorite { get; set; } = false;
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }

	public ItemGroup_cs? ItemGroup { get; set; }
	public ItemType_s? ItemType { get; set; }
	public UOM_cs? DefaultUOM { get; set; }
	public UOM_cs? DefaultPurchaseUOM { get; set; }
	public UOM_cs? DefaultSellingUOM { get; set; }
	public ICollection<ItemUOMConversion_cs> UOMConversions { get; set; } = [];
	public ICollection<Item_UOM_Barcode_cs> Barcodes { get; set; } = [];
	public ICollection<WarehouseTransaction> WarehouseTransactions { get; set; } = [];
	public ICollection<StockLedgerTransaction> StockLedgerTransactions { get; set; } = [];
}
