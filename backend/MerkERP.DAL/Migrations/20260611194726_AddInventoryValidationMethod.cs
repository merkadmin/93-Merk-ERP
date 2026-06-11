using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryValidationMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InventoryValidationMethodId",
                table: "Item_cs",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InventoryValidationMethod_s",
                columns: table => new
                {
                    InventoryValidationMethodId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryValidationMethod_s", x => x.InventoryValidationMethodId);
                });

            migrationBuilder.InsertData(
                table: "InventoryValidationMethod_s",
                columns: new[] { "InventoryValidationMethodId", "Name" },
                values: new object[,]
                {
                    { 1L, "FIFO" },
                    { 2L, "LIFO" },
                    { 3L, "Moving Average" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Item_cs_InventoryValidationMethodId",
                table: "Item_cs",
                column: "InventoryValidationMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_cs_InventoryValidationMethod_s_InventoryValidationMethodId",
                table: "Item_cs",
                column: "InventoryValidationMethodId",
                principalTable: "InventoryValidationMethod_s",
                principalColumn: "InventoryValidationMethodId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_cs_InventoryValidationMethod_s_InventoryValidationMethodId",
                table: "Item_cs");

            migrationBuilder.DropTable(
                name: "InventoryValidationMethod_s");

            migrationBuilder.DropIndex(
                name: "IX_Item_cs_InventoryValidationMethodId",
                table: "Item_cs");

            migrationBuilder.DropColumn(
                name: "InventoryValidationMethodId",
                table: "Item_cs");
        }
    }
}
