namespace MerkERP.Core.Models;

public class ItemType_s
{
    public int ItemTypeId { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Item_cs> Items { get; set; } = [];
}
