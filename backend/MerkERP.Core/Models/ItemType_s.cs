namespace MerkERP.Core.Models;

public class ItemType_s
{
    public long ItemTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsFavorite { get; set; } = false;
    public long? InsertedBy { get; set; }
    public DateTime? InsertedDate { get; set; }

    public ICollection<Item_cs> Items { get; set; } = [];
}
