namespace MerkERP.Core.Models;

public class TableMetaData
{
    public long    Id                 { get; set; }
    public int     TableNameId        { get; set; }
    public string  Key                { get; set; } = string.Empty;
    public string  LabelEN            { get; set; } = string.Empty;
    public string  LabelAR            { get; set; } = string.Empty;
    public int     Order              { get; set; }
    public string  EntityProperty     { get; set; } = string.Empty;
    public string? ForeignKeyProperty { get; set; }
    public string  FilterType         { get; set; } = "text";
    public string  DataType           { get; set; } = "string";
    public string  RenderAs           { get; set; } = "text";
    public bool    IsSortable         { get; set; } = true;
    public bool    IsFilterable       { get; set; } = true;
    public bool    IsVisible          { get; set; } = true;
    public int?    MinWidth           { get; set; }

    public TableName_s? TableName { get; set; }
}
