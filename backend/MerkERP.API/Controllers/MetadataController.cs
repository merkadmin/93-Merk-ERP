using Microsoft.AspNetCore.Mvc;
using MerkERP.API.Models;

namespace MerkERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetadataController : ControllerBase
{
    // ── Public endpoints ────────────────────────────────────────────────────────

    /// <summary>Returns column metadata for all known entities as a dictionary keyed by entityKey.</summary>
    [HttpGet]
    public IActionResult GetAll() => Ok(All.ToDictionary(e => e.EntityKey));

    /// <summary>Returns column metadata for a single entity.</summary>
    [HttpGet("{entity}")]
    public IActionResult Get(string entity)
    {
        var meta = All.FirstOrDefault(e =>
            string.Equals(e.EntityKey, entity, StringComparison.OrdinalIgnoreCase));
        return meta is not null ? Ok(meta) : NotFound(new { message = $"No metadata registered for '{entity}'." });
    }

    // ── Master list ─────────────────────────────────────────────────────────────

    private static readonly EntityMeta[] All =
    [
        new(
            EntityKey:  "items",
            DbTable:    "Item_cs",
            ModelClass: "Item_cs",
            Columns:
            [
                new("internalCode", "Code",        "الكود",        Order: 1, EntityProperty: "InternalCode"),
                new("name_AR",      "Name (AR)",   "الاسم (AR)",   Order: 2, EntityProperty: "Name_AR"),
                new("name_EN",      "Name (EN)",   "الاسم (EN)",   Order: 3, EntityProperty: "Name_EN"),
                new("itemGroup",    "Item Group",  "مجموعة الصنف", Order: 4, EntityProperty: "ItemGroup",  ForeignKeyProperty: "ItemGroupId",  FilterType: "select",  DataType: "string"),
                new("itemType",     "Item Type",   "نوع الصنف",    Order: 5, EntityProperty: "ItemType",   ForeignKeyProperty: "ItemTypeId",   FilterType: "select",  DataType: "string"),
                new("defaultUOM",   "Default UOM", "وحدة القياس",  Order: 6, EntityProperty: "DefaultUOM", ForeignKeyProperty: "DefaultUOMId", FilterType: "select",  DataType: "string"),
                new("isActive",     "Active",      "نشط",          Order: 7, EntityProperty: "IsActive",                                       FilterType: "boolean", DataType: "boolean", RenderAs: "badge"),
            ]
        ),

        new(
            EntityKey:  "itemgroups",
            DbTable:    "ItemGroup_cs",
            ModelClass: "ItemGroup_cs",
            Columns:
            [
                new("internalCode",    "Internal Code", "الكود الداخلي",  Order: 1, EntityProperty: "InternalCode"),
                new("name_AR",         "Name (AR)",     "الاسم (AR)",     Order: 2, EntityProperty: "Name_AR"),
                new("name_EN",         "Name (EN)",     "الاسم (EN)",     Order: 3, EntityProperty: "Name_EN"),
                new("parentItemGroup", "Parent Group",  "المجموعة الأصل", Order: 4, EntityProperty: "ParentItemGroup", ForeignKeyProperty: "ParentItemGroupId", FilterType: "select",  DataType: "string"),
                new("isMain",          "Is Main",       "أصل",            Order: 5, EntityProperty: "IsMain",                                                     FilterType: "boolean", DataType: "boolean", RenderAs: "badge"),
                new("isActive",        "Active",        "نشط",            Order: 6, EntityProperty: "IsActive",                                                   FilterType: "boolean", DataType: "boolean", RenderAs: "badge"),
            ]
        ),

        new(
            EntityKey:  "uoms",
            DbTable:    "UOM_cs",
            ModelClass: "UOM_cs",
            Columns:
            [
                new("internalCode",      "Internal Code",      "الكود الداخلي",      Order: 1, EntityProperty: "InternalCode"),
                new("name_AR",           "Name (AR)",          "الاسم (AR)",         Order: 2, EntityProperty: "Name_AR"),
                new("name_EN",           "Name (EN)",          "الاسم (EN)",         Order: 3, EntityProperty: "Name_EN"),
                new("mustBeWholeNumber", "Must Be Whole No.",  "يجب أن يكون صحيحاً", Order: 4, EntityProperty: "MustBeWholeNumber", FilterType: "boolean", DataType: "boolean", RenderAs: "yesno"),
                new("isActive",          "Active",             "نشط",                Order: 5, EntityProperty: "IsActive",          FilterType: "boolean", DataType: "boolean", RenderAs: "badge"),
            ]
        ),

        new(
            EntityKey:  "uomconversiongroups",
            DbTable:    "UOMConversionGroup_cs",
            ModelClass: "UOMConversionGroup_cs",
            Columns:
            [
                new("internalCode", "Internal Code", "الكود الداخلي", Order: 1, EntityProperty: "InternalCode"),
                new("name_AR",      "Name (AR)",     "الاسم (AR)",    Order: 2, EntityProperty: "Name_AR"),
                new("name_EN",      "Name (EN)",     "الاسم (EN)",    Order: 3, EntityProperty: "Name_EN"),
                new("isActive",     "Active",        "نشط",           Order: 4, EntityProperty: "IsActive", FilterType: "boolean", DataType: "boolean", RenderAs: "badge"),
            ]
        ),

        new(
            EntityKey:  "uomconversionfactors",
            DbTable:    "UOMConversionFactor_cs",
            ModelClass: "UOMConversionFactor_cs",
            Columns:
            [
                new("internalCode",       "Internal Code",    "الكود الداخلي",  Order: 1, EntityProperty: "InternalCode"),
                new("uomFrom",            "From UOM",         "من وحدة القياس",  Order: 2, EntityProperty: "UOMFrom",            ForeignKeyProperty: "UOMFromId",             FilterType: "select"),
                new("uomTo",              "To UOM",           "إلى وحدة القياس", Order: 3, EntityProperty: "UOMTo",              ForeignKeyProperty: "UOMToId",               FilterType: "select"),
                new("value",              "Factor",           "معامل التحويل",   Order: 4, EntityProperty: "Value",                                                           FilterType: "number",  DataType: "number"),
                new("uomConversionGroup", "Conversion Group", "مجموعة التحويل",  Order: 5, EntityProperty: "UOMConversionGroup", ForeignKeyProperty: "UOMConversionGroupId",  FilterType: "select"),
                new("isActive",           "Active",           "نشط",             Order: 6, EntityProperty: "IsActive",                                                        FilterType: "boolean", DataType: "boolean", RenderAs: "badge"),
            ]
        ),

        new(
            EntityKey:  "warehouses",
            DbTable:    "WareHouse_cs",
            ModelClass: "WareHouse_cs",
            Columns:
            [
                new("internalCode",      "Internal Code", "الكود الداخلي", Order: 1, EntityProperty: "InternalCode"),
                new("name_AR",           "Name (AR)",     "الاسم (AR)",    Order: 2, EntityProperty: "Name_AR"),
                new("name_EN",           "Name (EN)",     "الاسم (EN)",    Order: 3, EntityProperty: "Name_EN",           IsSortable: false, RenderAs: "tree"),
                new("wareHouseType",     "Type",          "النوع",         Order: 4, EntityProperty: "WareHouseType",     ForeignKeyProperty: "WareHouseTypeId",     FilterType: "select"),
                new("wareHouseCategory", "Category",      "الفئة",         Order: 5, EntityProperty: "WareHouseCategory", ForeignKeyProperty: "WareHouseCategoryId", FilterType: "select"),
                new("isParent",          "Is Parent",     "أصل",           Order: 6, EntityProperty: "IsParent",          FilterType: "boolean", DataType: "boolean", RenderAs: "badge"),
                new("isActive",          "Active",        "نشط",           Order: 7, EntityProperty: "IsActive",          FilterType: "boolean", DataType: "boolean", RenderAs: "badge"),
            ]
        ),

        new(
            EntityKey:  "warehousecategories",
            DbTable:    "WareHouseCategory_cs",
            ModelClass: "WareHouseCategory_cs",
            Columns:
            [
                new("internalCode", "Internal Code", "الكود الداخلي", Order: 1, EntityProperty: "InternalCode"),
                new("name_AR",      "Name (AR)",     "الاسم (AR)",    Order: 2, EntityProperty: "Name_AR"),
                new("name_EN",      "Name (EN)",     "الاسم (EN)",    Order: 3, EntityProperty: "Name_EN"),
                new("description",  "Description",  "الوصف",         Order: 4, EntityProperty: "Description", IsSortable: false),
                new("isActive",     "Active",        "نشط",           Order: 5, EntityProperty: "IsActive",    FilterType: "boolean", DataType: "boolean", RenderAs: "badge"),
            ]
        ),

        new(
            EntityKey:  "branches",
            DbTable:    "Branch_cs",
            ModelClass: "Branch_cs",
            Columns:
            [
                new("name_AR",     "Name (AR)",    "الاسم (AR)", Order: 1, EntityProperty: "Name_AR"),
                new("name_EN",     "Name (EN)",    "الاسم (EN)", Order: 2, EntityProperty: "Name_EN"),
                new("description", "Description", "الوصف",      Order: 3, EntityProperty: "Description", IsSortable: false),
                new("isActive",    "Active",       "نشط",        Order: 4, EntityProperty: "IsActive",    FilterType: "boolean", DataType: "boolean", RenderAs: "badge"),
            ]
        ),
    ];
}
