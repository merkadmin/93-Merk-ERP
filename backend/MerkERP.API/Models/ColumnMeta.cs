namespace MerkERP.API.Models;

/// <summary>
/// Describes a single table column's display and behaviour metadata,
/// including its mapping back to the physical entity property.
/// </summary>
/// <param name="Key">camelCase property name used in the frontend interface.</param>
/// <param name="LabelEN">English column header.</param>
/// <param name="LabelAR">Arabic column header.</param>
/// <param name="Order">1-based display order (RTL-aware; lower = rightmost in Arabic).</param>
/// <param name="EntityProperty">
///   Exact PascalCase property name on the C# model class.
///   For navigation / lookup columns this is the navigation property name
///   (e.g. "WareHouseCategory"), NOT the FK column.
/// </param>
/// <param name="ForeignKeyProperty">
///   The FK column name when <see cref="EntityProperty"/> is a navigation property
///   (e.g. "WareHouseCategoryId"). Null for direct scalar columns.
/// </param>
/// <param name="IsSortable">Whether the column supports server/client sort.</param>
/// <param name="IsFilterable">Whether the column appears in the search/filter bar.</param>
/// <param name="FilterType">Filter widget: "text" | "select" | "number" | "date" | "boolean" | "none".</param>
/// <param name="DataType">Underlying data type: "string" | "number" | "boolean" | "date".</param>
/// <param name="RenderAs">Rendering hint: "text" | "badge" | "tree" | "yesno".</param>
/// <param name="IsVisible">Default visibility; false = hidden but user-toggleable.</param>
/// <param name="MinWidth">Suggested minimum column width in pixels (null = auto).</param>
public record ColumnMeta(
    string  Key,
    string  LabelEN,
    string  LabelAR,
    int     Order,
    string  EntityProperty,
    string? ForeignKeyProperty = null,
    bool    IsSortable         = true,
    bool    IsFilterable       = true,
    string  FilterType         = "text",
    string  DataType           = "string",
    string  RenderAs           = "text",
    bool    IsVisible          = true,
    int?    MinWidth           = null
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
