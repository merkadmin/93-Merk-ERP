namespace MerkERP.Core.Models;

public class TableMetaData
{
    public long    Id                 { get; set; }
    public int     TableNameId        { get; set; }
    public string  Key                { get; set; } = string.Empty;
    public string  LabelEN            { get; set; } = string.Empty;
    public string  LabelAR            { get; set; } = string.Empty;
    public int     ColumnOrder        { get; set; }
    public string  EntityProperty     { get; set; } = string.Empty;
    public string? ForeignKeyProperty { get; set; }
    public int     FilterTypeId       { get; set; } = 1;
    public int     DataTypeId         { get; set; } = 1;
    public int     RenderAsId         { get; set; } = 1;
    public bool    IsSortable         { get; set; } = true;
    public bool    IsFilterable       { get; set; } = true;
    public bool    IsVisible          { get; set; } = true;
    public int?    MinWidth           { get; set; }

    public TableName_s?              TableName  { get; set; }
    public TableMetaData_FilterType_s? FilterType { get; set; }
    public TableMetaData_DataType_s?   DataType   { get; set; }
    public TableMetaData_RenderAs_s?   RenderAs   { get; set; }
}
