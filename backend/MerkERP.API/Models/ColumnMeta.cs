namespace MerkERP.API.Models;

/// <summary>
/// Describes a single table column's display and behaviour metadata,
/// including its mapping back to the physical entity property.
/// </summary>
/// <param name="Key">camelCase property name used in the frontend interface.</param>
/// <param name="LabelEN">English column header.</param>
/// <param name="LabelAR">Arabic column header.</param>
/// <param name="ColumnOrder">1-based display order (RTL-aware; lower = rightmost in Arabic).</param>
/// <param name="EntityProperty">
///   Exact PascalCase property name on the C# model class.
///   For navigation columns this is the navigation property (e.g. "WareHouseCategory"), not the FK.
/// </param>
/// <param name="ForeignKeyProperty">
///   The FK scalar column when EntityProperty is a navigation property (e.g. "WareHouseCategoryId").
///   Null for direct scalar columns.
/// </param>
/// <param name="FilterType">Filter widget: "text" | "select" | "number" | "date" | "boolean" | "none".</param>
/// <param name="DataType">Underlying data type: "string" | "number" | "boolean" | "date".</param>
/// <param name="RenderAs">Rendering hint: "text" | "badge" | "tree" | "yesno".</param>
/// <param name="IsSortable">Whether the column supports sort.</param>
/// <param name="IsFilterable">Whether the column appears in the filter bar.</param>
/// <param name="IsVisible">Default visibility; false = hidden but user-toggleable.</param>
/// <param name="MinWidth">Suggested minimum column width in pixels (null = auto).</param>
public record ColumnMeta(
    string  Key,
    string  LabelEN,
    string  LabelAR,
    int     ColumnOrder,
    string  EntityProperty,
    string? ForeignKeyProperty = null,   // 6
    string  FilterType         = "text", // 7
    string  DataType           = "string",// 8
    string  RenderAs           = "text", // 9
    bool    IsSortable         = true,   // 10
    bool    IsFilterable       = true,   // 11
    bool    IsVisible          = true,   // 12
    int?    MinWidth           = null    // 13
);

/// <summary>
/// Wraps column metadata for a whole entity together with its physical table mapping.
/// </summary>
/// <param name="EntityKey">Lowercase route key used in /api/metadata/{entityKey}.</param>
/// <param name="DbTable">SQL table name (matches the EF DbSet property name by convention).</param>
/// <param name="ModelClass">C# model class name (e.g. "WareHouse_cs").</param>
/// <param name="Columns">Ordered column definitions for this entity.</param>
public record EntityMeta(
    string       EntityKey,
    string       DbTable,
    string       ModelClass,
    ColumnMeta[] Columns
);
