using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseReceipt",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternalCode = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    SupplierId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint", nullable: false),
                    SupplierDeliveryNote = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    PostingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PostingTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    SetWarehouseId = table.Column<long>(type: "bigint", nullable: true),
                    StockTransactionStatusId = table.Column<long>(type: "bigint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalQty = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TaxTotal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    InsertedBy = table.Column<long>(type: "bigint", nullable: true),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseReceipt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseReceipt_Company_cs_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company_cs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseReceipt_Currency_s_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency_s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseReceipt_StockTransactionStatus_s_StockTransactionStatusId",
                        column: x => x.StockTransactionStatusId,
                        principalTable: "StockTransactionStatus_s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseReceipt_Supplier_cs_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier_cs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseReceipt_User_cs_InsertedBy",
                        column: x => x.InsertedBy,
                        principalTable: "User_cs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PurchaseReceipt_WareHouse_cs_SetWarehouseId",
                        column: x => x.SetWarehouseId,
                        principalTable: "WareHouse_cs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseReceiptItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseReceiptId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    UOMId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    InsertedBy = table.Column<long>(type: "bigint", nullable: true),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseReceiptItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseReceiptItem_Item_cs_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item_cs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseReceiptItem_PurchaseReceipt_PurchaseReceiptId",
                        column: x => x.PurchaseReceiptId,
                        principalTable: "PurchaseReceipt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseReceiptItem_UOM_cs_UOMId",
                        column: x => x.UOMId,
                        principalTable: "UOM_cs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseReceiptItem_User_cs_InsertedBy",
                        column: x => x.InsertedBy,
                        principalTable: "User_cs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseReceiptTax",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseReceiptId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(9,4)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    InsertedBy = table.Column<long>(type: "bigint", nullable: true),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseReceiptTax", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseReceiptTax_PurchaseReceipt_PurchaseReceiptId",
                        column: x => x.PurchaseReceiptId,
                        principalTable: "PurchaseReceipt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseReceiptTax_User_cs_InsertedBy",
                        column: x => x.InsertedBy,
                        principalTable: "User_cs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "TableName_s",
                columns: new[] { "Id", "EntityKey", "Name" },
                values: new object[] { 35, "purchase-receipts", "PurchaseReceipt" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceipt_CompanyId",
                table: "PurchaseReceipt",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceipt_CurrencyId",
                table: "PurchaseReceipt",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceipt_InsertedBy",
                table: "PurchaseReceipt",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceipt_InternalCode",
                table: "PurchaseReceipt",
                column: "InternalCode",
                unique: true,
                filter: "[InternalCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceipt_SetWarehouseId",
                table: "PurchaseReceipt",
                column: "SetWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceipt_StockTransactionStatusId",
                table: "PurchaseReceipt",
                column: "StockTransactionStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceipt_SupplierId",
                table: "PurchaseReceipt",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceiptItem_InsertedBy",
                table: "PurchaseReceiptItem",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceiptItem_ItemId",
                table: "PurchaseReceiptItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceiptItem_PurchaseReceiptId",
                table: "PurchaseReceiptItem",
                column: "PurchaseReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceiptItem_UOMId",
                table: "PurchaseReceiptItem",
                column: "UOMId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceiptTax_InsertedBy",
                table: "PurchaseReceiptTax",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceiptTax_PurchaseReceiptId",
                table: "PurchaseReceiptTax",
                column: "PurchaseReceiptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseReceiptItem");

            migrationBuilder.DropTable(
                name: "PurchaseReceiptTax");

            migrationBuilder.DropTable(
                name: "PurchaseReceipt");

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 35);
        }
    }
}
