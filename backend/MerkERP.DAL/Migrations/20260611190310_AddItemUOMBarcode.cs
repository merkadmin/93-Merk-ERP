using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddItemUOMBarcode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Item_UOM_Barcode_cs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    BarcodeTypeId = table.Column<long>(type: "bigint", nullable: false),
                    UOMId = table.Column<long>(type: "bigint", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item_UOM_Barcode_cs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Item_UOM_Barcode_cs_BarcodeType_s_BarcodeTypeId",
                        column: x => x.BarcodeTypeId,
                        principalTable: "BarcodeType_s",
                        principalColumn: "BarcodeTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Item_UOM_Barcode_cs_Item_cs_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item_cs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Item_UOM_Barcode_cs_UOM_cs_UOMId",
                        column: x => x.UOMId,
                        principalTable: "UOM_cs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Item_UOM_Barcode_cs_BarcodeTypeId",
                table: "Item_UOM_Barcode_cs",
                column: "BarcodeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_UOM_Barcode_cs_ItemId",
                table: "Item_UOM_Barcode_cs",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_UOM_Barcode_cs_UOMId",
                table: "Item_UOM_Barcode_cs",
                column: "UOMId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Item_UOM_Barcode_cs");
        }
    }
}
