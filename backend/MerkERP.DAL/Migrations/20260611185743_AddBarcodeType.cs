using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddBarcodeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BarcodeType_s",
                columns: table => new
                {
                    BarcodeTypeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarcodeType_s", x => x.BarcodeTypeId);
                });

            migrationBuilder.InsertData(
                table: "BarcodeType_s",
                columns: new[] { "BarcodeTypeId", "Name" },
                values: new object[,]
                {
                    { 1L, "EAN-13" },
                    { 2L, "EAN-8" },
                    { 3L, "UPC-A" },
                    { 4L, "UPC-E" },
                    { 5L, "Code 39" },
                    { 6L, "Code 128" },
                    { 7L, "ITF-14" },
                    { 8L, "GS1-128" },
                    { 9L, "QR Code" },
                    { 10L, "Data Matrix" },
                    { 11L, "Custom" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BarcodeType_s");
        }
    }
}
