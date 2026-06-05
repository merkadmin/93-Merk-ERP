namespace MerkERP.Core.Models;

public class ItemGroup_cs
{
	public long ItemGroupId { get; set; }
	public string Name { get; set; } = string.Empty;
	public long? ParentItemGroupId { get; set; }
	public bool IsGroup { get; set; }
	public bool IsActive { get; set; } = true;
	public bool IsFavorite { get; set; } = false;
	public long? InsertedBy { get; set; }
	public DateTime? InsertedDate { get; set; }

	public ItemGroup_cs? ParentItemGroup { get; set; }
	public ICollection<ItemGroup_cs> Children { get; set; } = [];
	public ICollection<Item_cs> Items { get; set; } = [];
}
