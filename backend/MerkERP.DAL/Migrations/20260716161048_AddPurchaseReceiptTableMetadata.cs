using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseReceiptTableMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Grid column definitions for the "purchase-receipts" list page (TableNameId 35 = PurchaseReceipt).
            // FilterType: text=1, number=2, boolean=3, select=4
            // DataType:   string=1, number=2, boolean=3, date=4
            // RenderAs:   text=1, badge=2, yesno=3, tree=4
            migrationBuilder.InsertData(
                table: "TableMetaData",
                columns: new[] { "Id", "TableNameId", "Key", "LabelEN", "LabelAR", "ColumnOrder", "EntityProperty", "ForeignKeyProperty", "FilterTypeId", "DataTypeId", "RenderAsId", "IsSortable", "IsFilterable", "IsVisible", "MinWidth" },
                values: new object[,]
                {
                    { 69L, 35, "internalCode",           "Internal Code",  "الكود الداخلي", 1, "InternalCode",           null,                        1, 1, 1, true, true, true, null },
                    { 70L, 35, "supplier",                "Supplier",       "المورد",        2, "Supplier",               "SupplierId",                4, 1, 1, true, true, true, null },
                    { 71L, 35, "postingDate",             "Posting Date",   "تاريخ الترحيل",  3, "PostingDate",            null,                        1, 4, 1, true, true, true, null },
                    { 72L, 35, "company",                 "Company",        "الشركة",        4, "Company",                "CompanyId",                 4, 1, 1, true, true, true, null },
                    { 73L, 35, "setWarehouse",            "Warehouse",      "المخزن",        5, "SetWarehouse",           "SetWarehouseId",            4, 1, 1, true, true, true, null },
                    { 74L, 35, "grandTotal",              "Grand Total",    "الإجمالي الكلي", 6, "GrandTotal",             null,                        2, 2, 1, true, true, true, null },
                    { 75L, 35, "stockTransactionStatus",  "Status",         "الحالة",        7, "StockTransactionStatus", "StockTransactionStatusId",  4, 1, 1, true, true, true, null },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM TableMetaData WHERE Id BETWEEN 69 AND 75;");
        }
    }
}
