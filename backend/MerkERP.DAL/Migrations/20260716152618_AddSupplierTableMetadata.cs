using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierTableMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Grid column definitions for the "suppliers" list page (TableNameId 32 = Supplier_cs).
            // FilterType: text=1, number=2, boolean=3, select=4
            // DataType:   string=1, number=2, boolean=3, date=4
            // RenderAs:   text=1, badge=2, yesno=3, tree=4
            migrationBuilder.InsertData(
                table: "TableMetaData",
                columns: new[] { "Id", "TableNameId", "Key", "LabelEN", "LabelAR", "ColumnOrder", "EntityProperty", "ForeignKeyProperty", "FilterTypeId", "DataTypeId", "RenderAsId", "IsSortable", "IsFilterable", "IsVisible", "MinWidth" },
                values: new object[,]
                {
                    { 53L, 32, "internalCode", "Internal Code",  "الكود الداخلي", 1, "InternalCode", null,               1, 1, 1, true, true, true, null },
                    { 54L, 32, "name_EN",      "Name (EN)",      "الاسم (EN)",    2, "Name_EN",      null,               1, 1, 1, true, true, true, null },
                    { 55L, 32, "name_AR",      "Name (AR)",      "الاسم (AR)",    3, "Name_AR",      null,               1, 1, 1, true, true, true, null },
                    { 56L, 32, "supplierType", "Supplier Type",  "نوع المورد",    4, "SupplierType", "SupplierTypeId",   4, 1, 1, true, true, true, null },
                    { 57L, 32, "country",      "Country",        "الدولة",        5, "Country",      null,               1, 1, 1, true, true, true, null },
                    { 58L, 32, "phone",        "Phone",          "الهاتف",        6, "Phone",        null,               1, 1, 1, true, true, true, null },
                    { 59L, 32, "isOnHold",     "On Hold",        "موقوف",         7, "IsOnHold",     null,               3, 3, 2, true, true, true, null },
                    { 60L, 32, "isActive",     "Active",         "نشط",           8, "IsActive",     null,               3, 3, 2, true, true, true, null },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM TableMetaData WHERE Id BETWEEN 53 AND 60;");
        }
    }
}
