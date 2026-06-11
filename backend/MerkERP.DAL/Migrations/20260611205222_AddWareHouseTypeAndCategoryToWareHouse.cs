using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddWareHouseTypeAndCategoryToWareHouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InternalCode",
                table: "WareHouse_cs",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WareHouseCategoryId",
                table: "WareHouse_cs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WareHouseTypeId",
                table: "WareHouse_cs",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WareHouse_cs_WareHouseCategoryId",
                table: "WareHouse_cs",
                column: "WareHouseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_WareHouse_cs_WareHouseTypeId",
                table: "WareHouse_cs",
                column: "WareHouseTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_WareHouse_cs_WareHouseCategory_cs_WareHouseCategoryId",
                table: "WareHouse_cs",
                column: "WareHouseCategoryId",
                principalTable: "WareHouseCategory_cs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WareHouse_cs_WareHouseType_s_WareHouseTypeId",
                table: "WareHouse_cs",
                column: "WareHouseTypeId",
                principalTable: "WareHouseType_s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WareHouse_cs_WareHouseCategory_cs_WareHouseCategoryId",
                table: "WareHouse_cs");

            migrationBuilder.DropForeignKey(
                name: "FK_WareHouse_cs_WareHouseType_s_WareHouseTypeId",
                table: "WareHouse_cs");

            migrationBuilder.DropIndex(
                name: "IX_WareHouse_cs_WareHouseCategoryId",
                table: "WareHouse_cs");

            migrationBuilder.DropIndex(
                name: "IX_WareHouse_cs_WareHouseTypeId",
                table: "WareHouse_cs");

            migrationBuilder.DropColumn(
                name: "InternalCode",
                table: "WareHouse_cs");

            migrationBuilder.DropColumn(
                name: "WareHouseCategoryId",
                table: "WareHouse_cs");

            migrationBuilder.DropColumn(
                name: "WareHouseTypeId",
                table: "WareHouse_cs");
        }
    }
}
