using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToStockReconciliationTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE StockReconciliationTransaction
                    ADD StockTransactionStatusId BIGINT NOT NULL
                        CONSTRAINT DF_SRT_Status DEFAULT 1;

                ALTER TABLE StockReconciliationTransaction
                    ADD CONSTRAINT FK_SRT_StockTransactionStatus
                        FOREIGN KEY (StockTransactionStatusId)
                        REFERENCES StockTransactionStatus_s(Id);

                CREATE INDEX IX_SRT_StockTransactionStatusId
                    ON StockReconciliationTransaction (StockTransactionStatusId);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS IX_SRT_StockTransactionStatusId ON StockReconciliationTransaction;
                ALTER TABLE StockReconciliationTransaction DROP CONSTRAINT FK_SRT_StockTransactionStatus;
                ALTER TABLE StockReconciliationTransaction DROP CONSTRAINT DF_SRT_Status;
                ALTER TABLE StockReconciliationTransaction DROP COLUMN StockTransactionStatusId;
            ");
        }
    }
}
