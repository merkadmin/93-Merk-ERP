using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddStockTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- StockTransactionType_s (no IDENTITY — managed with explicit IDs)
                CREATE TABLE StockTransactionType_s (
                    Id      BIGINT        NOT NULL,
                    Name_EN NVARCHAR(200) NOT NULL,
                    Name_AR NVARCHAR(200) NOT NULL,
                    CONSTRAINT PK_StockTransactionType_s PRIMARY KEY (Id)
                );

                DECLARE @t1 NVARCHAR(200) = N'رصيد افتتاحي';
                DECLARE @t2 NVARCHAR(200) = N'جرد المخزون';
                DECLARE @t3 NVARCHAR(200) = N'استلام مخزون';
                DECLARE @t4 NVARCHAR(200) = N'صرف مخزون';
                DECLARE @t5 NVARCHAR(200) = N'تحويل مخزون';

                INSERT INTO StockTransactionType_s (Id, Name_EN, Name_AR) VALUES
                    (1, 'Opening Balance',      @t1),
                    (2, 'Stock Reconciliation', @t2),
                    (3, 'Stock Receipt',        @t3),
                    (4, 'Stock Issue',          @t4),
                    (5, 'Stock Transfer',       @t5);

                -- StockReconciliationTransaction
                CREATE TABLE StockReconciliationTransaction (
                    Id                      BIGINT        IDENTITY(1,1) NOT NULL,
                    StockTransactionTypeId  BIGINT        NOT NULL,
                    InternalCode            NVARCHAR(50)  NULL,
                    PostingDate             DATE          NOT NULL,
                    PostingTime             TIME          NOT NULL,
                    SetWarehouseId          BIGINT        NULL,
                    InsertedBy              BIGINT        NULL,
                    InsertedDate            DATETIME2     NULL,
                    CONSTRAINT PK_StockReconciliationTransaction PRIMARY KEY (Id),
                    CONSTRAINT FK_SRT_StockTransactionType  FOREIGN KEY (StockTransactionTypeId) REFERENCES StockTransactionType_s(Id),
                    CONSTRAINT FK_SRT_SetWarehouse          FOREIGN KEY (SetWarehouseId)         REFERENCES WareHouse_cs(Id),
                    CONSTRAINT FK_SRT_InsertedBy            FOREIGN KEY (InsertedBy)             REFERENCES User_cs(Id) ON DELETE SET NULL
                );
                CREATE INDEX IX_SRT_StockTransactionTypeId ON StockReconciliationTransaction (StockTransactionTypeId);
                CREATE INDEX IX_SRT_SetWarehouseId         ON StockReconciliationTransaction (SetWarehouseId);
                CREATE INDEX IX_SRT_InsertedBy             ON StockReconciliationTransaction (InsertedBy);

                -- StockReconciliationTransactionDetail
                CREATE TABLE StockReconciliationTransactionDetail (
                    Id                              BIGINT         IDENTITY(1,1) NOT NULL,
                    StockReconciliationTransactionId BIGINT        NOT NULL,
                    ItemId                          BIGINT         NOT NULL,
                    WarehouseId                     BIGINT         NOT NULL,
                    Quantity                        DECIMAL(18,4)  NOT NULL,
                    UOMId                           BIGINT         NOT NULL,
                    InsertedBy                      BIGINT         NULL,
                    InsertedDate                    DATETIME2      NULL,
                    CONSTRAINT PK_StockReconciliationTransactionDetail PRIMARY KEY (Id),
                    CONSTRAINT FK_SRTD_SRT        FOREIGN KEY (StockReconciliationTransactionId) REFERENCES StockReconciliationTransaction(Id) ON DELETE CASCADE,
                    CONSTRAINT FK_SRTD_Item       FOREIGN KEY (ItemId)      REFERENCES Item_cs(Id),
                    CONSTRAINT FK_SRTD_Warehouse  FOREIGN KEY (WarehouseId) REFERENCES WareHouse_cs(Id),
                    CONSTRAINT FK_SRTD_UOM        FOREIGN KEY (UOMId)       REFERENCES UOM_cs(Id),
                    CONSTRAINT FK_SRTD_InsertedBy FOREIGN KEY (InsertedBy)  REFERENCES User_cs(Id) ON DELETE SET NULL
                );
                CREATE INDEX IX_SRTD_StockReconciliationTransactionId ON StockReconciliationTransactionDetail (StockReconciliationTransactionId);
                CREATE INDEX IX_SRTD_ItemId      ON StockReconciliationTransactionDetail (ItemId);
                CREATE INDEX IX_SRTD_WarehouseId ON StockReconciliationTransactionDetail (WarehouseId);
                CREATE INDEX IX_SRTD_UOMId       ON StockReconciliationTransactionDetail (UOMId);
                CREATE INDEX IX_SRTD_InsertedBy  ON StockReconciliationTransactionDetail (InsertedBy);

                -- StockLedgerTransaction
                CREATE TABLE StockLedgerTransaction (
                    Id                       BIGINT        IDENTITY(1,1) NOT NULL,
                    StockTransactionTypeId   BIGINT        NOT NULL,
                    InternalCode             NVARCHAR(50)  NULL,
                    ItemId                   BIGINT        NOT NULL,
                    WareHouseId              BIGINT        NOT NULL,
                    ActualQuantity           DECIMAL(18,4) NOT NULL,
                    QuantityAfterTransaction DECIMAL(18,4) NOT NULL,
                    ValuationRate            DECIMAL(18,4) NOT NULL,
                    InsertedBy               BIGINT        NULL,
                    InsertedDate             DATETIME2     NULL,
                    CONSTRAINT PK_StockLedgerTransaction PRIMARY KEY (Id),
                    CONSTRAINT FK_SLT_StockTransactionType FOREIGN KEY (StockTransactionTypeId) REFERENCES StockTransactionType_s(Id),
                    CONSTRAINT FK_SLT_Item                 FOREIGN KEY (ItemId)      REFERENCES Item_cs(Id),
                    CONSTRAINT FK_SLT_WareHouse            FOREIGN KEY (WareHouseId) REFERENCES WareHouse_cs(Id),
                    CONSTRAINT FK_SLT_InsertedBy           FOREIGN KEY (InsertedBy)  REFERENCES User_cs(Id) ON DELETE SET NULL
                );
                CREATE INDEX IX_SLT_StockTransactionTypeId ON StockLedgerTransaction (StockTransactionTypeId);
                CREATE INDEX IX_SLT_ItemId      ON StockLedgerTransaction (ItemId);
                CREATE INDEX IX_SLT_WareHouseId ON StockLedgerTransaction (WareHouseId);
                CREATE INDEX IX_SLT_InsertedBy  ON StockLedgerTransaction (InsertedBy);

                -- TableName_s registry entries
                INSERT INTO TableName_s (Id, Name, EntityKey) VALUES
                    (21, 'StockTransactionType_s',               NULL),
                    (22, 'StockReconciliationTransaction',       NULL),
                    (23, 'StockReconciliationTransactionDetail', NULL),
                    (24, 'StockLedgerTransaction',               NULL);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM TableName_s WHERE Id IN (21, 22, 23, 24);
                DROP TABLE IF EXISTS StockLedgerTransaction;
                DROP TABLE IF EXISTS StockReconciliationTransactionDetail;
                DROP TABLE IF EXISTS StockReconciliationTransaction;
                DROP TABLE IF EXISTS StockTransactionType_s;
            ");
        }
    }
}
