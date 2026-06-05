namespace MerkERP.Core.Models;

public class ItemGroup_cs
{
    public int ItemGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentItemGroupId { get; set; }
    public bool IsGroup { get; set; }
    public bool IsActive { get; set; } = true;

    public ItemGroup_cs? ParentItemGroup { get; set; }
    public ICollection<ItemGroup_cs> Children { get; set; } = [];
    public ICollection<Item_cs> Items { get; set; } = [];
}
