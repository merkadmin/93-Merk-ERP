using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddWareHouseTypeIdToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "WareHouseTypeId",
                table: "WareHouseCategory_cs",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 12L,
                column: "RenderAs",
                value: "badge");

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 34L,
                column: "RenderAs",
                value: "badge");

            migrationBuilder.CreateIndex(
                name: "IX_WareHouseCategory_cs_WareHouseTypeId",
                table: "WareHouseCategory_cs",
                column: "WareHouseTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_WareHouseCategory_cs_WareHouseType_s_WareHouseTypeId",
                table: "WareHouseCategory_cs",
                column: "WareHouseTypeId",
                principalTable: "WareHouseType_s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WareHouseCategory_cs_WareHouseType_s_WareHouseTypeId",
                table: "WareHouseCategory_cs");

            migrationBuilder.DropIndex(
                name: "IX_WareHouseCategory_cs_WareHouseTypeId",
                table: "WareHouseCategory_cs");

            migrationBuilder.DropColumn(
                name: "WareHouseTypeId",
                table: "WareHouseCategory_cs");

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 12L,
                column: "RenderAs",
                value: "yesno");

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 34L,
                column: "RenderAs",
                value: "yesno");
        }
    }
}
