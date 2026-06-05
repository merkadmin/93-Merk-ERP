using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MerkERP.DAL.Migrations
{
	/// <inheritdoc />
	public partial class InitialCreate : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "ItemGroup_cs",
				columns: table => new
				{
					ItemGroupId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ParentItemGroupId = table.Column<int>(type: "int", nullable: true),
					IsGroup = table.Column<bool>(type: "bit", nullable: false),
					IsActive = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ItemGroup_cs", x => x.ItemGroupId);
					table.ForeignKey(
						name: "FK_ItemGroup_cs_ItemGroup_cs_ParentItemGroupId",
						column: x => x.ParentItemGroupId,
						principalTable: "ItemGroup_cs",
						principalColumn: "ItemGroupId",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "ItemType_s",
				columns: table => new
				{
					ItemTypeId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ItemType_s", x => x.ItemTypeId);
				});

			migrationBuilder.CreateTable(
				name: "UOM_cs",
				columns: table => new
				{
					UOMId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
					MustBeWholeNumber = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_UOM_cs", x => x.UOMId);
				});

			migrationBuilder.CreateTable(
				name: "WareHouse_cs",
				columns: table => new
				{
					WarehouseId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ParentWarehouseId = table.Column<int>(type: "int", nullable: true),
					IsGroup = table.Column<bool>(type: "bit", nullable: false),
					IsActive = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_WareHouse_cs", x => x.WarehouseId);
					table.ForeignKey(
						name: "FK_WareHouse_cs_WareHouse_cs_ParentWarehouseId",
						column: x => x.ParentWarehouseId,
						principalTable: "WareHouse_cs",
						principalColumn: "WarehouseId",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "Item_cs",
				columns: table => new
				{
					ItemId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					ItemCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
					ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ItemGroupId = table.Column<int>(type: "int", nullable: false),
					ItemTypeId = table.Column<int>(type: "int", nullable: false),
					DefaultUOMId = table.Column<int>(type: "int", nullable: false),
					Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
					HasBatch = table.Column<bool>(type: "bit", nullable: false),
					HasSerial = table.Column<bool>(type: "bit", nullable: false),
					IsActive = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Item_cs", x => x.ItemId);
					table.ForeignKey(
						name: "FK_Item_cs_ItemGroup_cs_ItemGroupId",
						column: x => x.ItemGroupId,
						principalTable: "ItemGroup_cs",
						principalColumn: "ItemGroupId",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_Item_cs_ItemType_s_ItemTypeId",
						column: x => x.ItemTypeId,
						principalTable: "ItemType_s",
						principalColumn: "ItemTypeId",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_Item_cs_UOM_cs_DefaultUOMId",
						column: x => x.DefaultUOMId,
						principalTable: "UOM_cs",
						principalColumn: "UOMId",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "ItemUOMConversion_cs",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					ItemId = table.Column<int>(type: "int", nullable: false),
					UOMId = table.Column<int>(type: "int", nullable: false),
					ConversionFactor = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ItemUOMConversion_cs", x => x.Id);
					table.ForeignKey(
						name: "FK_ItemUOMConversion_cs_Item_cs_ItemId",
						column: x => x.ItemId,
						principalTable: "Item_cs",
						principalColumn: "ItemId",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_ItemUOMConversion_cs_UOM_cs_UOMId",
						column: x => x.UOMId,
						principalTable: "UOM_cs",
						principalColumn: "UOMId",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "StockLedgerTransaction",
				columns: table => new
				{
					SLTId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					ItemId = table.Column<int>(type: "int", nullable: false),
					WarehouseId = table.Column<int>(type: "int", nullable: false),
					PostingDate = table.Column<DateOnly>(type: "date", nullable: false),
					PostingTime = table.Column<TimeOnly>(type: "time", nullable: false),
					ActualQty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					QtyAfterTransaction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					ValuationRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					StockValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					VoucherType = table.Column<string>(type: "nvarchar(max)", nullable: false),
					VoucherNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
					BatchNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
					CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_StockLedgerTransaction", x => x.SLTId);
					table.ForeignKey(
						name: "FK_StockLedgerTransaction_Item_cs_ItemId",
						column: x => x.ItemId,
						principalTable: "Item_cs",
						principalColumn: "ItemId",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_StockLedgerTransaction_WareHouse_cs_WarehouseId",
						column: x => x.WarehouseId,
						principalTable: "WareHouse_cs",
						principalColumn: "WarehouseId",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "WarehouseTransaction",
				columns: table => new
				{
					BinId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					ItemId = table.Column<int>(type: "int", nullable: false),
					WarehouseId = table.Column<int>(type: "int", nullable: false),
					ActualQty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					ReservedQty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					OrderedQty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					ValuationRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_WarehouseTransaction", x => x.BinId);
					table.ForeignKey(
						name: "FK_WarehouseTransaction_Item_cs_ItemId",
						column: x => x.ItemId,
						principalTable: "Item_cs",
						principalColumn: "ItemId",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_WarehouseTransaction_WareHouse_cs_WarehouseId",
						column: x => x.WarehouseId,
						principalTable: "WareHouse_cs",
						principalColumn: "WarehouseId",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.InsertData(
				table: "ItemType_s",
				columns: new[] { "ItemTypeId", "Name" },
				values: new object[,]
				{
					{ 1, "Stock Item" },
					{ 2, "Service" },
					{ 3, "Non-Stock Item" }
				});

			migrationBuilder.CreateIndex(
				name: "IX_Item_cs_DefaultUOMId",
				table: "Item_cs",
				column: "DefaultUOMId");

			migrationBuilder.CreateIndex(
				name: "IX_Item_cs_ItemCode",
				table: "Item_cs",
				column: "ItemCode",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Item_cs_ItemGroupId",
				table: "Item_cs",
				column: "ItemGroupId");

			migrationBuilder.CreateIndex(
				name: "IX_Item_cs_ItemTypeId",
				table: "Item_cs",
				column: "ItemTypeId");

			migrationBuilder.CreateIndex(
				name: "IX_ItemGroup_cs_ParentItemGroupId",
				table: "ItemGroup_cs",
				column: "ParentItemGroupId");

			migrationBuilder.CreateIndex(
				name: "IX_ItemUOMConversion_cs_ItemId",
				table: "ItemUOMConversion_cs",
				column: "ItemId");

			migrationBuilder.CreateIndex(
				name: "IX_ItemUOMConversion_cs_UOMId",
				table: "ItemUOMConversion_cs",
				column: "UOMId");

			migrationBuilder.CreateIndex(
				name: "IX_StockLedgerTransaction_ItemId",
				table: "StockLedgerTransaction",
				column: "ItemId");

			migrationBuilder.CreateIndex(
				name: "IX_StockLedgerTransaction_WarehouseId",
				table: "StockLedgerTransaction",
				column: "WarehouseId");

			migrationBuilder.CreateIndex(
				name: "IX_WareHouse_cs_ParentWarehouseId",
				table: "WareHouse_cs",
				column: "ParentWarehouseId");

			migrationBuilder.CreateIndex(
				name: "IX_WarehouseTransaction_ItemId_WarehouseId",
				table: "WarehouseTransaction",
				columns: new[] { "ItemId", "WarehouseId" },
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_WarehouseTransaction_WarehouseId",
				table: "WarehouseTransaction",
				column: "WarehouseId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "ItemUOMConversion_cs");

			migrationBuilder.DropTable(
				name: "StockLedgerTransaction");

			migrationBuilder.DropTable(
				name: "WarehouseTransaction");

			migrationBuilder.DropTable(
				name: "Item_cs");

			migrationBuilder.DropTable(
				name: "WareHouse_cs");

			migrationBuilder.DropTable(
				name: "ItemGroup_cs");

			migrationBuilder.DropTable(
				name: "ItemType_s");

			migrationBuilder.DropTable(
				name: "UOM_cs");
		}
	}
}
