using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStockTransactionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF OBJECT_ID('StockLedgerTransaction', 'U') IS NOT NULL DROP TABLE StockLedgerTransaction;
                IF OBJECT_ID('WarehouseTransaction', 'U') IS NOT NULL DROP TABLE WarehouseTransaction;
                DELETE FROM TableName_s WHERE Id IN (9, 17);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restoration not implemented — tables were intentionally removed
        }
    }
}
