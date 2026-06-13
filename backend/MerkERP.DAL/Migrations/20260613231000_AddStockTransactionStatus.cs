using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddStockTransactionStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE StockTransactionStatus_s (
                    Id      BIGINT        NOT NULL,
                    Name_EN NVARCHAR(200) NOT NULL,
                    Name_AR NVARCHAR(200) NOT NULL,
                    CONSTRAINT PK_StockTransactionStatus_s PRIMARY KEY (Id)
                );

                DECLARE @s1 NVARCHAR(200) = N'مسودة';
                DECLARE @s2 NVARCHAR(200) = N'قيد الانتظار';
                DECLARE @s3 NVARCHAR(200) = N'مُقدَّم';
                DECLARE @s4 NVARCHAR(200) = N'ملغى';
                DECLARE @s5 NVARCHAR(200) = N'معدَّل';

                INSERT INTO StockTransactionStatus_s (Id, Name_EN, Name_AR) VALUES
                    (1, 'Draft',     @s1),
                    (2, 'Pending',   @s2),
                    (3, 'Submitted', @s3),
                    (4, 'Cancelled', @s4),
                    (5, 'Amended',   @s5);

                INSERT INTO TableName_s (Id, Name, EntityKey)
                VALUES (25, 'StockTransactionStatus_s', NULL);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM TableName_s WHERE Id = 25;
                DROP TABLE IF EXISTS StockTransactionStatus_s;
            ");
        }
    }
}
