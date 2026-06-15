using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    public partial class AddStockReconciliationTransactionDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE StockReconciliationTransactionDate (
                    Id                              BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    StockReconciliationTransactionId BIGINT NOT NULL,
                    ChangeDateTime                  DATETIME2 NOT NULL,
                    StockTransactionStatusId        BIGINT NOT NULL,
                    StockTransactionTypeId          BIGINT NOT NULL,
                    InsertedBy                      BIGINT NULL,

                    CONSTRAINT FK_SRTD_Transaction
                        FOREIGN KEY (StockReconciliationTransactionId)
                        REFERENCES StockReconciliationTransaction(Id) ON DELETE CASCADE,

                    CONSTRAINT FK_SRTD_Status
                        FOREIGN KEY (StockTransactionStatusId)
                        REFERENCES StockTransactionStatus_s(Id),

                    CONSTRAINT FK_SRTD_Type
                        FOREIGN KEY (StockTransactionTypeId)
                        REFERENCES StockTransactionType_s(Id),

                    CONSTRAINT FK_SRTD_User
                        FOREIGN KEY (InsertedBy)
                        REFERENCES User_cs(Id) ON DELETE SET NULL
                );

                CREATE INDEX IX_SRTD_TransactionId ON StockReconciliationTransactionDate (StockReconciliationTransactionId);
                CREATE INDEX IX_SRTD_InsertedBy    ON StockReconciliationTransactionDate (InsertedBy);
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TABLE IF EXISTS StockReconciliationTransactionDate;
            ");
        }
    }
}
