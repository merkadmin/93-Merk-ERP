using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    public partial class AddGetTableMetaDataProc : Migration
    {
        protected override void Up(MigrationBuilder m)
        {
            m.Sql(@"
CREATE OR ALTER PROCEDURE sp_GetTableMetaData
    @TableNameId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        tm.Id,
        tm.TableNameId,
        tn.Name          AS TableName,
        tn.EntityKey,
        tm.[Key],
        tm.LabelEN,
        tm.LabelAR,
        tm.[Order],
        tm.EntityProperty,
        tm.ForeignKeyProperty,
        tm.FilterType,
        tm.DataType,
        tm.RenderAs,
        tm.IsSortable,
        tm.IsFilterable,
        tm.IsVisible,
        tm.MinWidth
    FROM TableMetaData tm
    INNER JOIN TableName_s tn ON tn.Id = tm.TableNameId
    WHERE tm.TableNameId = @TableNameId
    ORDER BY tm.[Order];
END
");
        }

        protected override void Down(MigrationBuilder m)
        {
            m.Sql("DROP PROCEDURE IF EXISTS sp_GetTableMetaData;");
        }
    }
}
