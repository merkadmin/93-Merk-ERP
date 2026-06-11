using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHasBatchHasSerialFromItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasBatch",
                table: "Item_cs");

            migrationBuilder.DropColumn(
                name: "HasSerial",
                table: "Item_cs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasBatch",
                table: "Item_cs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasSerial",
                table: "Item_cs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
