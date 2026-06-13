using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddWareHouseTypeMetaToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TableMetaData",
                columns: new[] { "Id", "ColumnOrder", "DataType", "EntityProperty", "FilterType", "ForeignKeyProperty", "IsFilterable", "IsSortable", "IsVisible", "Key", "LabelAR", "LabelEN", "MinWidth", "RenderAs", "TableNameId" },
                values: new object[] { 45L, 6, "string", "WareHouseType", "select", "WareHouseTypeId", true, true, true, "wareHouseType", "النوع", "Type", null, "text", 15 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 45L);
        }
    }
}
