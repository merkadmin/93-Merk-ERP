namespace MerkERP.API.Models;

public class ColumnMetaRow
{
    public long    Id                  { get; init; }
    public int     TableNameId         { get; init; }
    public string  TableName           { get; init; } = "";
    public string  EntityKey           { get; init; } = "";
    public string  Key                 { get; init; } = "";
    public string  LabelEN             { get; init; } = "";
    public string  LabelAR             { get; init; } = "";
    public int     ColumnOrder         { get; init; }
    public string  EntityProperty      { get; init; } = "";
    public string? ForeignKeyProperty  { get; init; }
    public string  FilterType          { get; init; } = "text";
    public string  DataType            { get; init; } = "string";
    public string  RenderAs            { get; init; } = "text";
    public bool    IsSortable          { get; init; }
    public bool    IsFilterable        { get; init; }
    public bool    IsVisible           { get; init; }
    public int?    MinWidth            { get; init; }
}
