using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTableMetaData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntityKey",
                table: "TableName_s",
                type: "nvarchar(100)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TableMetaData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    TableNameId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    LabelEN = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    LabelAR = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    EntityProperty = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ForeignKeyProperty = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    FilterType = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    RenderAs = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    IsSortable = table.Column<bool>(type: "bit", nullable: false),
                    IsFilterable = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    MinWidth = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableMetaData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableMetaData_TableName_s_TableNameId",
                        column: x => x.TableNameId,
                        principalTable: "TableName_s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "TableName_s",
                columns: new[] { "Id", "EntityKey", "Name" },
                values: new object[,]
                {
                    { 1, null, "BarcodeType_s" },
                    { 2, "branches", "Branch_cs" },
                    { 3, null, "InventoryValidationMethod_s" },
                    { 4, "items", "Item_cs" },
                    { 5, null, "Item_UOM_Barcode_cs" },
                    { 6, "itemgroups", "ItemGroup_cs" },
                    { 7, null, "ItemType_s" },
                    { 8, null, "ItemUOMConversion_cs" },
                    { 9, null, "StockLedgerTransaction" },
                    { 10, null, "TableName_s" },
                    { 11, "uoms", "UOM_cs" },
                    { 12, "uomconversionfactors", "UOMConversionFactor_cs" },
                    { 13, "uomconversiongroups", "UOMConversionGroup_cs" },
                    { 14, "warehouses", "WareHouse_cs" },
                    { 15, "warehousecategories", "WareHouseCategory_cs" },
                    { 16, null, "WareHouseType_s" },
                    { 17, null, "WarehouseTransaction" },
                    { 18, null, "TableMetaData" }
                });

            migrationBuilder.InsertData(
                table: "TableMetaData",
                columns: new[] { "Id", "DataType", "EntityProperty", "FilterType", "ForeignKeyProperty", "IsFilterable", "IsSortable", "IsVisible", "Key", "LabelAR", "LabelEN", "MinWidth", "Order", "RenderAs", "TableNameId" },
                values: new object[,]
                {
                    { 1L, "string", "InternalCode", "text", null, true, true, true, "internalCode", "الكود", "Code", null, 1, "text", 4 },
                    { 2L, "string", "Name_AR", "text", null, true, true, true, "name_AR", "الاسم (AR)", "Name (AR)", null, 2, "text", 4 },
                    { 3L, "string", "Name_EN", "text", null, true, true, true, "name_EN", "الاسم (EN)", "Name (EN)", null, 3, "text", 4 },
                    { 4L, "string", "ItemGroup", "select", "ItemGroupId", true, true, true, "itemGroup", "مجموعة الصنف", "Item Group", null, 4, "text", 4 },
                    { 5L, "string", "ItemType", "select", "ItemTypeId", true, true, true, "itemType", "نوع الصنف", "Item Type", null, 5, "text", 4 },
                    { 6L, "string", "DefaultUOM", "select", "DefaultUOMId", true, true, true, "defaultUOM", "وحدة القياس", "Default UOM", null, 6, "text", 4 },
                    { 7L, "boolean", "IsActive", "boolean", null, true, true, true, "isActive", "نشط", "Active", null, 7, "badge", 4 },
                    { 8L, "string", "InternalCode", "text", null, true, true, true, "internalCode", "الكود الداخلي", "Internal Code", null, 1, "text", 6 },
                    { 9L, "string", "Name_AR", "text", null, true, true, true, "name_AR", "الاسم (AR)", "Name (AR)", null, 2, "text", 6 },
                    { 10L, "string", "Name_EN", "text", null, true, true, true, "name_EN", "الاسم (EN)", "Name (EN)", null, 3, "text", 6 },
                    { 11L, "string", "ParentItemGroup", "select", "ParentItemGroupId", true, true, true, "parentItemGroup", "المجموعة الأصل", "Parent Group", null, 4, "text", 6 },
                    { 12L, "boolean", "IsMain", "boolean", null, true, true, true, "isMain", "أصل", "Is Main", null, 5, "badge", 6 },
                    { 13L, "boolean", "IsActive", "boolean", null, true, true, true, "isActive", "نشط", "Active", null, 6, "badge", 6 },
                    { 14L, "string", "InternalCode", "text", null, true, true, true, "internalCode", "الكود الداخلي", "Internal Code", null, 1, "text", 11 },
                    { 15L, "string", "Name_AR", "text", null, true, true, true, "name_AR", "الاسم (AR)", "Name (AR)", null, 2, "text", 11 },
                    { 16L, "string", "Name_EN", "text", null, true, true, true, "name_EN", "الاسم (EN)", "Name (EN)", null, 3, "text", 11 },
                    { 17L, "boolean", "MustBeWholeNumber", "boolean", null, true, true, true, "mustBeWholeNumber", "يجب أن يكون صحيحاً", "Must Be Whole No.", null, 4, "yesno", 11 },
                    { 18L, "boolean", "IsActive", "boolean", null, true, true, true, "isActive", "نشط", "Active", null, 5, "badge", 11 },
                    { 19L, "string", "InternalCode", "text", null, true, true, true, "internalCode", "الكود الداخلي", "Internal Code", null, 1, "text", 13 },
                    { 20L, "string", "Name_AR", "text", null, true, true, true, "name_AR", "الاسم (AR)", "Name (AR)", null, 2, "text", 13 },
                    { 21L, "string", "Name_EN", "text", null, true, true, true, "name_EN", "الاسم (EN)", "Name (EN)", null, 3, "text", 13 },
                    { 22L, "boolean", "IsActive", "boolean", null, true, true, true, "isActive", "نشط", "Active", null, 4, "badge", 13 },
                    { 23L, "string", "InternalCode", "text", null, true, true, true, "internalCode", "الكود الداخلي", "Internal Code", null, 1, "text", 12 },
                    { 24L, "string", "UOMFrom", "select", "UOMFromId", true, true, true, "uomFrom", "من وحدة القياس", "From UOM", null, 2, "text", 12 },
                    { 25L, "string", "UOMTo", "select", "UOMToId", true, true, true, "uomTo", "إلى وحدة القياس", "To UOM", null, 3, "text", 12 },
                    { 26L, "number", "Value", "number", null, true, true, true, "value", "معامل التحويل", "Factor", null, 4, "text", 12 },
                    { 27L, "string", "UOMConversionGroup", "select", "UOMConversionGroupId", true, true, true, "uomConversionGroup", "مجموعة التحويل", "Conversion Group", null, 5, "text", 12 },
                    { 28L, "boolean", "IsActive", "boolean", null, true, true, true, "isActive", "نشط", "Active", null, 6, "badge", 12 },
                    { 29L, "string", "InternalCode", "text", null, true, true, true, "internalCode", "الكود الداخلي", "Internal Code", null, 1, "text", 14 },
                    { 30L, "string", "Name_AR", "text", null, true, true, true, "name_AR", "الاسم (AR)", "Name (AR)", null, 2, "text", 14 },
                    { 31L, "string", "Name_EN", "text", null, true, false, true, "name_EN", "الاسم (EN)", "Name (EN)", null, 3, "tree", 14 },
                    { 32L, "string", "WareHouseType", "select", "WareHouseTypeId", true, true, true, "wareHouseType", "النوع", "Type", null, 4, "text", 14 },
                    { 33L, "string", "WareHouseCategory", "select", "WareHouseCategoryId", true, true, true, "wareHouseCategory", "الفئة", "Category", null, 5, "text", 14 },
                    { 34L, "boolean", "IsParent", "boolean", null, true, true, true, "isParent", "أصل", "Is Parent", null, 6, "badge", 14 },
                    { 35L, "boolean", "IsActive", "boolean", null, true, true, true, "isActive", "نشط", "Active", null, 7, "badge", 14 },
                    { 36L, "string", "InternalCode", "text", null, true, true, true, "internalCode", "الكود الداخلي", "Internal Code", null, 1, "text", 15 },
                    { 37L, "string", "Name_AR", "text", null, true, true, true, "name_AR", "الاسم (AR)", "Name (AR)", null, 2, "text", 15 },
                    { 38L, "string", "Name_EN", "text", null, true, true, true, "name_EN", "الاسم (EN)", "Name (EN)", null, 3, "text", 15 },
                    { 39L, "string", "Description", "text", null, true, false, true, "description", "الوصف", "Description", null, 4, "text", 15 },
                    { 40L, "boolean", "IsActive", "boolean", null, true, true, true, "isActive", "نشط", "Active", null, 5, "badge", 15 },
                    { 41L, "string", "Name_AR", "text", null, true, true, true, "name_AR", "الاسم (AR)", "Name (AR)", null, 1, "text", 2 },
                    { 42L, "string", "Name_EN", "text", null, true, true, true, "name_EN", "الاسم (EN)", "Name (EN)", null, 2, "text", 2 },
                    { 43L, "string", "Description", "text", null, true, false, true, "description", "الوصف", "Description", null, 3, "text", 2 },
                    { 44L, "boolean", "IsActive", "boolean", null, true, true, true, "isActive", "نشط", "Active", null, 4, "badge", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TableMetaData_TableNameId_Key",
                table: "TableMetaData",
                columns: new[] { "TableNameId", "Key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TableMetaData");

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DropColumn(
                name: "EntityKey",
                table: "TableName_s");
        }
    }
}
