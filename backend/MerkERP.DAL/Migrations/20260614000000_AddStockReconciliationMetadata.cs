using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    public partial class AddStockReconciliationMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE TableName_s
                SET EntityKey = 'stockreconciliationtransactions'
                WHERE Id = 22;

                INSERT INTO TableMetaData
                    (Id, TableNameId, [Key], LabelEN, LabelAR, [Order],
                     EntityProperty, ForeignKeyProperty,
                     FilterType, DataType, RenderAs,
                     IsSortable, IsFilterable, IsVisible, MinWidth)
                VALUES
                (47, 22, 'internalCode',          'Internal Code',    N'الكود الداخلي',       1, 'InternalCode',          NULL,                    'text',   'string', 'text', 1, 1, 1, NULL),
                (48, 22, 'stockTransactionType',   'Transaction Type', N'نوع المعاملة',        2, 'StockTransactionType',   'StockTransactionTypeId', 'select', 'string', 'text', 1, 1, 1, NULL),
                (49, 22, 'stockTransactionStatus', 'Status',           N'الحالة',              3, 'StockTransactionStatus', 'StockTransactionStatusId','select','string', 'text', 1, 1, 1, NULL),
                (50, 22, 'postingDate',            'Posting Date',     N'تاريخ الترحيل',       4, 'PostingDate',           NULL,                    'text',   'string', 'text', 1, 0, 1, NULL),
                (51, 22, 'setWarehouse',           'Set Warehouse',    N'المستودع المحدد',     5, 'SetWarehouse',          'SetWarehouseId',         'select', 'string', 'text', 1, 1, 1, NULL);
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE TableName_s SET EntityKey = NULL WHERE Id = 22;
                DELETE FROM TableMetaData WHERE Id IN (47, 48, 49, 50, 51);
            ");
        }
    }
}
