using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    public partial class AddIsActiveToStockReconciliationTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE StockReconciliationTransaction
                    ADD IsActive BIT NOT NULL
                        CONSTRAINT DF_SRT_IsActive DEFAULT 1;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE StockReconciliationTransaction DROP CONSTRAINT DF_SRT_IsActive;
                ALTER TABLE StockReconciliationTransaction DROP COLUMN IsActive;
            ");
        }
    }
}
