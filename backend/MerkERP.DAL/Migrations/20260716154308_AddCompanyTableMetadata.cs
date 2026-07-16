using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyTableMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Grid column definitions for the "companies" list page (TableNameId 33 = Company_cs).
            // FilterType: text=1, number=2, boolean=3, select=4
            // DataType:   string=1, number=2, boolean=3, date=4
            // RenderAs:   text=1, badge=2, yesno=3, tree=4
            migrationBuilder.InsertData(
                table: "TableMetaData",
                columns: new[] { "Id", "TableNameId", "Key", "LabelEN", "LabelAR", "ColumnOrder", "EntityProperty", "ForeignKeyProperty", "FilterTypeId", "DataTypeId", "RenderAsId", "IsSortable", "IsFilterable", "IsVisible", "MinWidth" },
                values: new object[,]
                {
                    { 61L, 33, "internalCode",    "Internal Code",   "الكود الداخلي", 1, "InternalCode",    null,              1, 1, 1, true, true, true, null },
                    { 62L, 33, "name_EN",         "Name (EN)",       "الاسم (EN)",    2, "Name_EN",         null,              1, 1, 1, true, true, true, null },
                    { 63L, 33, "name_AR",         "Name (AR)",       "الاسم (AR)",    3, "Name_AR",         null,              1, 1, 1, true, true, true, null },
                    { 64L, 33, "abbr",            "Abbr",            "الاختصار",      4, "Abbr",            null,              1, 1, 1, true, true, true, null },
                    { 65L, 33, "defaultCurrency", "Default Currency","العملة الافتراضية", 5, "DefaultCurrency", "DefaultCurrencyId", 4, 1, 1, true, true, true, null },
                    { 66L, 33, "country",         "Country",         "الدولة",        6, "Country",         null,              1, 1, 1, true, true, true, null },
                    { 67L, 33, "phone",           "Phone",           "الهاتف",        7, "Phone",           null,              1, 1, 1, true, true, true, null },
                    { 68L, 33, "isActive",        "Active",          "نشط",           8, "IsActive",        null,              3, 3, 2, true, true, true, null },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM TableMetaData WHERE Id BETWEEN 61 AND 68;");
        }
    }
}
