using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameWarehouseFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "WareHouse_cs");

            migrationBuilder.RenameColumn(
                name: "WarehouseId",
                table: "WareHouse_cs",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "WareHouse_cs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name_AR",
                table: "WareHouse_cs",
                type: "nvarchar(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name_EN",
                table: "WareHouse_cs",
                type: "nvarchar(200)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "WareHouse_cs");

            migrationBuilder.DropColumn(
                name: "Name_AR",
                table: "WareHouse_cs");

            migrationBuilder.DropColumn(
                name: "Name_EN",
                table: "WareHouse_cs");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "WareHouse_cs",
                newName: "WarehouseId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "WareHouse_cs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
