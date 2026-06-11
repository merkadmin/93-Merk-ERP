using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddItemDefaultWarehouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DefaultWarehouseId",
                table: "Item_cs",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Item_cs_DefaultWarehouseId",
                table: "Item_cs",
                column: "DefaultWarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_cs_WareHouse_cs_DefaultWarehouseId",
                table: "Item_cs",
                column: "DefaultWarehouseId",
                principalTable: "WareHouse_cs",
                principalColumn: "WarehouseId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_cs_WareHouse_cs_DefaultWarehouseId",
                table: "Item_cs");

            migrationBuilder.DropIndex(
                name: "IX_Item_cs_DefaultWarehouseId",
                table: "Item_cs");

            migrationBuilder.DropColumn(
                name: "DefaultWarehouseId",
                table: "Item_cs");
        }
    }
}
