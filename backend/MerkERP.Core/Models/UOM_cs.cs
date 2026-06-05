namespace MerkERP.Core.Models;

public class UOM_cs
{
    public int UOMId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool MustBeWholeNumber { get; set; }

    public ICollection<Item_cs> DefaultForItems { get; set; } = [];
    public ICollection<ItemUOMConversion_cs> Conversions { get; set; } = [];
}
