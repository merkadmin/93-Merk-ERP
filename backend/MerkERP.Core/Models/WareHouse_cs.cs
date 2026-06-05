namespace MerkERP.Core.Models;

public class WareHouse_cs
{
    public int WarehouseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentWarehouseId { get; set; }
    public bool IsGroup { get; set; }
    public bool IsActive { get; set; } = true;

    public WareHouse_cs? ParentWarehouse { get; set; }
    public ICollection<WareHouse_cs> Children { get; set; } = [];
    public ICollection<WarehouseTransaction> Transactions { get; set; } = [];
    public ICollection<StockLedgerTransaction> StockLedgerTransactions { get; set; } = [];
}
