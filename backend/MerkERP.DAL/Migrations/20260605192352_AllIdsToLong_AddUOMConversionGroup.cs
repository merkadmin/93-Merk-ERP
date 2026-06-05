using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AllIdsToLong_AddUOMConversionGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── 1. Drop all foreign keys ──────────────────────────────────────────
            migrationBuilder.DropForeignKey("FK_WarehouseTransaction_Item_cs_ItemId",           "WarehouseTransaction");
            migrationBuilder.DropForeignKey("FK_WarehouseTransaction_WareHouse_cs_WarehouseId", "WarehouseTransaction");
            migrationBuilder.DropForeignKey("FK_StockLedgerTransaction_Item_cs_ItemId",         "StockLedgerTransaction");
            migrationBuilder.DropForeignKey("FK_StockLedgerTransaction_WareHouse_cs_WarehouseId","StockLedgerTransaction");
            migrationBuilder.DropForeignKey("FK_ItemUOMConversion_cs_Item_cs_ItemId",           "ItemUOMConversion_cs");
            migrationBuilder.DropForeignKey("FK_ItemUOMConversion_cs_UOM_cs_UOMId",             "ItemUOMConversion_cs");
            migrationBuilder.DropForeignKey("FK_Item_cs_ItemGroup_cs_ItemGroupId",              "Item_cs");
            migrationBuilder.DropForeignKey("FK_Item_cs_ItemType_s_ItemTypeId",                 "Item_cs");
            migrationBuilder.DropForeignKey("FK_Item_cs_UOM_cs_DefaultUOMId",                  "Item_cs");
            migrationBuilder.DropForeignKey("FK_ItemGroup_cs_ItemGroup_cs_ParentItemGroupId",   "ItemGroup_cs");
            migrationBuilder.DropForeignKey("FK_WareHouse_cs_WareHouse_cs_ParentWarehouseId",   "WareHouse_cs");

            // ── 2. Drop indexes on columns being altered ──────────────────────────
            migrationBuilder.DropIndex("IX_WarehouseTransaction_ItemId_WarehouseId", "WarehouseTransaction");
            migrationBuilder.DropIndex("IX_WarehouseTransaction_WarehouseId",        "WarehouseTransaction");
            migrationBuilder.DropIndex("IX_StockLedgerTransaction_ItemId",           "StockLedgerTransaction");
            migrationBuilder.DropIndex("IX_StockLedgerTransaction_WarehouseId",      "StockLedgerTransaction");
            migrationBuilder.DropIndex("IX_ItemUOMConversion_cs_ItemId",             "ItemUOMConversion_cs");
            migrationBuilder.DropIndex("IX_ItemUOMConversion_cs_UOMId",              "ItemUOMConversion_cs");
            migrationBuilder.DropIndex("IX_Item_cs_DefaultUOMId",                   "Item_cs");
            migrationBuilder.DropIndex("IX_Item_cs_ItemGroupId",                    "Item_cs");
            migrationBuilder.DropIndex("IX_Item_cs_ItemTypeId",                     "Item_cs");
            migrationBuilder.DropIndex("IX_ItemGroup_cs_ParentItemGroupId",         "ItemGroup_cs");
            migrationBuilder.DropIndex("IX_WareHouse_cs_ParentWarehouseId",         "WareHouse_cs");

            // ── 3. Drop primary keys (cannot alter a PK column while PK exists) ──
            migrationBuilder.DropPrimaryKey("PK_WarehouseTransaction", "WarehouseTransaction");
            migrationBuilder.DropPrimaryKey("PK_WareHouse_cs",         "WareHouse_cs");
            migrationBuilder.DropPrimaryKey("PK_UOM_cs",               "UOM_cs");
            migrationBuilder.DropPrimaryKey("PK_StockLedgerTransaction","StockLedgerTransaction");
            migrationBuilder.DropPrimaryKey("PK_ItemUOMConversion_cs", "ItemUOMConversion_cs");
            migrationBuilder.DropPrimaryKey("PK_ItemType_s",           "ItemType_s");
            migrationBuilder.DropPrimaryKey("PK_ItemGroup_cs",         "ItemGroup_cs");
            migrationBuilder.DropPrimaryKey("PK_Item_cs",              "Item_cs");

            // ── 4. Delete seed data (uses int key, must delete before altering) ──
            migrationBuilder.DeleteData("ItemType_s", "ItemTypeId", 1);
            migrationBuilder.DeleteData("ItemType_s", "ItemTypeId", 2);
            migrationBuilder.DeleteData("ItemType_s", "ItemTypeId", 3);

            // ── 5. Alter all ID columns int → bigint ──────────────────────────────
            // WarehouseTransaction
            migrationBuilder.AlterColumn<long>("WarehouseId", "WarehouseTransaction", "bigint", nullable: false, oldClrType: typeof(int), oldType: "int");
            migrationBuilder.AlterColumn<long>("ItemId",      "WarehouseTransaction", "bigint", nullable: false, oldClrType: typeof(int), oldType: "int");
            migrationBuilder.AlterColumn<long>("BinId",       "WarehouseTransaction", "bigint", nullable: false, oldClrType: typeof(int), oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1").OldAnnotation("SqlServer:Identity", "1, 1");
            // WareHouse_cs
            migrationBuilder.AlterColumn<long>("ParentWarehouseId", "WareHouse_cs", "bigint", nullable: true,  oldClrType: typeof(int), oldType: "int", oldNullable: true);
            migrationBuilder.AlterColumn<long>("WarehouseId",       "WareHouse_cs", "bigint", nullable: false, oldClrType: typeof(int), oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1").OldAnnotation("SqlServer:Identity", "1, 1");
            // UOM_cs
            migrationBuilder.AlterColumn<long>("UOMId", "UOM_cs", "bigint", nullable: false, oldClrType: typeof(int), oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1").OldAnnotation("SqlServer:Identity", "1, 1");
            // StockLedgerTransaction
            migrationBuilder.AlterColumn<long>("WarehouseId", "StockLedgerTransaction", "bigint", nullable: false, oldClrType: typeof(int), oldType: "int");
            migrationBuilder.AlterColumn<long>("ItemId",      "StockLedgerTransaction", "bigint", nullable: false, oldClrType: typeof(int), oldType: "int");
            migrationBuilder.AlterColumn<long>("SLTId",       "StockLedgerTransaction", "bigint", nullable: false, oldClrType: typeof(int), oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1").OldAnnotation("SqlServer:Identity", "1, 1");
            // ItemUOMConversion_cs
            migrationBuilder.AlterColumn<long>("UOMId",  "ItemUOMConversion_cs", "bigint", nullable: false, oldClrType: typeof(int), oldType: "int");
            migrationBuilder.AlterColumn<long>("ItemId", "ItemUOMConversion_cs", "bigint", nullable: false, oldClrType: typeof(int), oldType: "int");
            migrationBuilder.AlterColumn<long>("Id",     "ItemUOMConversion_cs", "bigint", nullable: false, oldClrType: typeof(int), oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1").OldAnnotation("SqlServer:Identity", "1, 1");
            // ItemType_s
            migrationBuilder.AlterColumn<long>("ItemTypeId", "ItemType_s", "bigint", nullable: false, oldClrType: typeof(int), oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1").OldAnnotation("SqlServer:Identity", "1, 1");
            // ItemGroup_cs
            migrationBuilder.AlterColumn<long>("ParentItemGroupId", "ItemGroup_cs", "bigint", nullable: true,  oldClrType: typeof(int), oldType: "int", oldNullable: true);
            migrationBuilder.AlterColumn<long>("ItemGroupId",       "ItemGroup_cs", "bigint", nullable: false, oldClrType: typeof(int), oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1").OldAnnotation("SqlServer:Identity", "1, 1");
            // Item_cs
            migrationBuilder.AlterColumn<long>("ItemTypeId",   "Item_cs", "bigint", nullable: false, oldClrType: typeof(int), oldType: "int");
            migrationBuilder.AlterColumn<long>("ItemGroupId",  "Item_cs", "bigint", nullable: false, oldClrType: typeof(int), oldType: "int");
            migrationBuilder.AlterColumn<long>("DefaultUOMId", "Item_cs", "bigint", nullable: false, oldClrType: typeof(int), oldType: "int");
            migrationBuilder.AlterColumn<long>("ItemId",       "Item_cs", "bigint", nullable: false, oldClrType: typeof(int), oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1").OldAnnotation("SqlServer:Identity", "1, 1");

            // ── 6. Recreate primary keys ──────────────────────────────────────────
            migrationBuilder.AddPrimaryKey("PK_WarehouseTransaction",  "WarehouseTransaction",  "BinId");
            migrationBuilder.AddPrimaryKey("PK_WareHouse_cs",          "WareHouse_cs",          "WarehouseId");
            migrationBuilder.AddPrimaryKey("PK_UOM_cs",                "UOM_cs",                "UOMId");
            migrationBuilder.AddPrimaryKey("PK_StockLedgerTransaction","StockLedgerTransaction","SLTId");
            migrationBuilder.AddPrimaryKey("PK_ItemUOMConversion_cs",  "ItemUOMConversion_cs",  "Id");
            migrationBuilder.AddPrimaryKey("PK_ItemType_s",            "ItemType_s",            "ItemTypeId");
            migrationBuilder.AddPrimaryKey("PK_ItemGroup_cs",          "ItemGroup_cs",          "ItemGroupId");
            migrationBuilder.AddPrimaryKey("PK_Item_cs",               "Item_cs",               "ItemId");

            // ── 7. Restore seed data ──────────────────────────────────────────────
            migrationBuilder.InsertData(
                table: "ItemType_s",
                columns: new[] { "ItemTypeId", "Name" },
                values: new object[,]
                {
                    { 1L, "Stock Item" },
                    { 2L, "Service" },
                    { 3L, "Non-Stock Item" }
                });

            // ── 8. Recreate indexes ───────────────────────────────────────────────
            migrationBuilder.CreateIndex("IX_Item_cs_DefaultUOMId",              "Item_cs",               "DefaultUOMId");
            migrationBuilder.CreateIndex("IX_Item_cs_ItemGroupId",               "Item_cs",               "ItemGroupId");
            migrationBuilder.CreateIndex("IX_Item_cs_ItemTypeId",                "Item_cs",               "ItemTypeId");
            migrationBuilder.CreateIndex("IX_ItemGroup_cs_ParentItemGroupId",    "ItemGroup_cs",          "ParentItemGroupId");
            migrationBuilder.CreateIndex("IX_ItemUOMConversion_cs_ItemId",       "ItemUOMConversion_cs",  "ItemId");
            migrationBuilder.CreateIndex("IX_ItemUOMConversion_cs_UOMId",        "ItemUOMConversion_cs",  "UOMId");
            migrationBuilder.CreateIndex("IX_StockLedgerTransaction_ItemId",     "StockLedgerTransaction","ItemId");
            migrationBuilder.CreateIndex("IX_StockLedgerTransaction_WarehouseId","StockLedgerTransaction","WarehouseId");
            migrationBuilder.CreateIndex("IX_WareHouse_cs_ParentWarehouseId",    "WareHouse_cs",          "ParentWarehouseId");
            migrationBuilder.CreateIndex("IX_WarehouseTransaction_WarehouseId",  "WarehouseTransaction",  "WarehouseId");
            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransaction_ItemId_WarehouseId",
                table: "WarehouseTransaction",
                columns: new[] { "ItemId", "WarehouseId" },
                unique: true);

            // ── 9. Recreate foreign keys ──────────────────────────────────────────
            migrationBuilder.AddForeignKey("FK_Item_cs_ItemGroup_cs_ItemGroupId",               "Item_cs",               "ItemGroupId",       "ItemGroup_cs",          principalColumn: "ItemGroupId",  onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_Item_cs_ItemType_s_ItemTypeId",                  "Item_cs",               "ItemTypeId",        "ItemType_s",            principalColumn: "ItemTypeId",   onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_Item_cs_UOM_cs_DefaultUOMId",                    "Item_cs",               "DefaultUOMId",      "UOM_cs",                principalColumn: "UOMId",        onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_ItemGroup_cs_ItemGroup_cs_ParentItemGroupId",    "ItemGroup_cs",          "ParentItemGroupId", "ItemGroup_cs",          principalColumn: "ItemGroupId",  onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_ItemUOMConversion_cs_Item_cs_ItemId",            "ItemUOMConversion_cs",  "ItemId",            "Item_cs",               principalColumn: "ItemId",       onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_ItemUOMConversion_cs_UOM_cs_UOMId",              "ItemUOMConversion_cs",  "UOMId",             "UOM_cs",                principalColumn: "UOMId",        onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_StockLedgerTransaction_Item_cs_ItemId",          "StockLedgerTransaction","ItemId",            "Item_cs",               principalColumn: "ItemId",       onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_StockLedgerTransaction_WareHouse_cs_WarehouseId","StockLedgerTransaction","WarehouseId",       "WareHouse_cs",          principalColumn: "WarehouseId",  onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_WareHouse_cs_WareHouse_cs_ParentWarehouseId",    "WareHouse_cs",          "ParentWarehouseId", "WareHouse_cs",          principalColumn: "WarehouseId",  onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_WarehouseTransaction_Item_cs_ItemId",            "WarehouseTransaction",  "ItemId",            "Item_cs",               principalColumn: "ItemId",       onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_WarehouseTransaction_WareHouse_cs_WarehouseId",  "WarehouseTransaction",  "WarehouseId",       "WareHouse_cs",          principalColumn: "WarehouseId",  onDelete: ReferentialAction.Restrict);

            // ── 10. Create new UOMConversionGroup_cs table ────────────────────────
            migrationBuilder.CreateTable(
                name: "UOMConversionGroup_cs",
                columns: table => new
                {
                    Id        = table.Column<long>(type: "bigint", nullable: false)
                                    .Annotation("SqlServer:Identity", "1, 1"),
                    UOMFromId = table.Column<long>(type: "bigint", nullable: false),
                    UOMToId   = table.Column<long>(type: "bigint", nullable: false),
                    Value     = table.Column<double>(type: "float",  nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UOMConversionGroup_cs", x => x.Id);
                    table.ForeignKey("FK_UOMConversionGroup_cs_UOM_cs_UOMFromId", x => x.UOMFromId, "UOM_cs", "UOMId", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_UOMConversionGroup_cs_UOM_cs_UOMToId",   x => x.UOMToId,   "UOM_cs", "UOMId", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex("IX_UOMConversionGroup_cs_UOMFromId", "UOMConversionGroup_cs", "UOMFromId");
            migrationBuilder.CreateIndex("IX_UOMConversionGroup_cs_UOMToId",   "UOMConversionGroup_cs", "UOMToId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("UOMConversionGroup_cs");

            migrationBuilder.DropForeignKey("FK_WarehouseTransaction_Item_cs_ItemId",           "WarehouseTransaction");
            migrationBuilder.DropForeignKey("FK_WarehouseTransaction_WareHouse_cs_WarehouseId", "WarehouseTransaction");
            migrationBuilder.DropForeignKey("FK_StockLedgerTransaction_Item_cs_ItemId",         "StockLedgerTransaction");
            migrationBuilder.DropForeignKey("FK_StockLedgerTransaction_WareHouse_cs_WarehouseId","StockLedgerTransaction");
            migrationBuilder.DropForeignKey("FK_ItemUOMConversion_cs_Item_cs_ItemId",           "ItemUOMConversion_cs");
            migrationBuilder.DropForeignKey("FK_ItemUOMConversion_cs_UOM_cs_UOMId",             "ItemUOMConversion_cs");
            migrationBuilder.DropForeignKey("FK_Item_cs_ItemGroup_cs_ItemGroupId",              "Item_cs");
            migrationBuilder.DropForeignKey("FK_Item_cs_ItemType_s_ItemTypeId",                 "Item_cs");
            migrationBuilder.DropForeignKey("FK_Item_cs_UOM_cs_DefaultUOMId",                  "Item_cs");
            migrationBuilder.DropForeignKey("FK_ItemGroup_cs_ItemGroup_cs_ParentItemGroupId",   "ItemGroup_cs");
            migrationBuilder.DropForeignKey("FK_WareHouse_cs_WareHouse_cs_ParentWarehouseId",   "WareHouse_cs");

            migrationBuilder.DropIndex("IX_WarehouseTransaction_ItemId_WarehouseId", "WarehouseTransaction");
            migrationBuilder.DropIndex("IX_WarehouseTransaction_WarehouseId",        "WarehouseTransaction");
            migrationBuilder.DropIndex("IX_StockLedgerTransaction_ItemId",           "StockLedgerTransaction");
            migrationBuilder.DropIndex("IX_StockLedgerTransaction_WarehouseId",      "StockLedgerTransaction");
            migrationBuilder.DropIndex("IX_ItemUOMConversion_cs_ItemId",             "ItemUOMConversion_cs");
            migrationBuilder.DropIndex("IX_ItemUOMConversion_cs_UOMId",              "ItemUOMConversion_cs");
            migrationBuilder.DropIndex("IX_Item_cs_DefaultUOMId",                   "Item_cs");
            migrationBuilder.DropIndex("IX_Item_cs_ItemGroupId",                    "Item_cs");
            migrationBuilder.DropIndex("IX_Item_cs_ItemTypeId",                     "Item_cs");
            migrationBuilder.DropIndex("IX_ItemGroup_cs_ParentItemGroupId",         "ItemGroup_cs");
            migrationBuilder.DropIndex("IX_WareHouse_cs_ParentWarehouseId",         "WareHouse_cs");

            migrationBuilder.DropPrimaryKey("PK_WarehouseTransaction", "WarehouseTransaction");
            migrationBuilder.DropPrimaryKey("PK_WareHouse_cs",         "WareHouse_cs");
            migrationBuilder.DropPrimaryKey("PK_UOM_cs",               "UOM_cs");
            migrationBuilder.DropPrimaryKey("PK_StockLedgerTransaction","StockLedgerTransaction");
            migrationBuilder.DropPrimaryKey("PK_ItemUOMConversion_cs", "ItemUOMConversion_cs");
            migrationBuilder.DropPrimaryKey("PK_ItemType_s",           "ItemType_s");
            migrationBuilder.DropPrimaryKey("PK_ItemGroup_cs",         "ItemGroup_cs");
            migrationBuilder.DropPrimaryKey("PK_Item_cs",              "Item_cs");

            migrationBuilder.DeleteData("ItemType_s", "ItemTypeId", 1L);
            migrationBuilder.DeleteData("ItemType_s", "ItemTypeId", 2L);
            migrationBuilder.DeleteData("ItemType_s", "ItemTypeId", 3L);

            migrationBuilder.AlterColumn<int>("WarehouseId", "WarehouseTransaction", "int", nullable: false, oldClrType: typeof(long), oldType: "bigint");
            migrationBuilder.AlterColumn<int>("ItemId",      "WarehouseTransaction", "int", nullable: false, oldClrType: typeof(long), oldType: "bigint");
            migrationBuilder.AlterColumn<int>("BinId",       "WarehouseTransaction", "int", nullable: false, oldClrType: typeof(long), oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1").OldAnnotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AlterColumn<int>("ParentWarehouseId", "WareHouse_cs", "int", nullable: true,  oldClrType: typeof(long), oldType: "bigint", oldNullable: true);
            migrationBuilder.AlterColumn<int>("WarehouseId",       "WareHouse_cs", "int", nullable: false, oldClrType: typeof(long), oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1").OldAnnotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AlterColumn<int>("UOMId", "UOM_cs", "int", nullable: false, oldClrType: typeof(long), oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1").OldAnnotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AlterColumn<int>("WarehouseId", "StockLedgerTransaction", "int", nullable: false, oldClrType: typeof(long), oldType: "bigint");
            migrationBuilder.AlterColumn<int>("ItemId",      "StockLedgerTransaction", "int", nullable: false, oldClrType: typeof(long), oldType: "bigint");
            migrationBuilder.AlterColumn<int>("SLTId",       "StockLedgerTransaction", "int", nullable: false, oldClrType: typeof(long), oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1").OldAnnotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AlterColumn<int>("UOMId",  "ItemUOMConversion_cs", "int", nullable: false, oldClrType: typeof(long), oldType: "bigint");
            migrationBuilder.AlterColumn<int>("ItemId", "ItemUOMConversion_cs", "int", nullable: false, oldClrType: typeof(long), oldType: "bigint");
            migrationBuilder.AlterColumn<int>("Id",     "ItemUOMConversion_cs", "int", nullable: false, oldClrType: typeof(long), oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1").OldAnnotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AlterColumn<int>("ItemTypeId", "ItemType_s", "int", nullable: false, oldClrType: typeof(long), oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1").OldAnnotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AlterColumn<int>("ParentItemGroupId", "ItemGroup_cs", "int", nullable: true,  oldClrType: typeof(long), oldType: "bigint", oldNullable: true);
            migrationBuilder.AlterColumn<int>("ItemGroupId",       "ItemGroup_cs", "int", nullable: false, oldClrType: typeof(long), oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1").OldAnnotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AlterColumn<int>("ItemTypeId",   "Item_cs", "int", nullable: false, oldClrType: typeof(long), oldType: "bigint");
            migrationBuilder.AlterColumn<int>("ItemGroupId",  "Item_cs", "int", nullable: false, oldClrType: typeof(long), oldType: "bigint");
            migrationBuilder.AlterColumn<int>("DefaultUOMId", "Item_cs", "int", nullable: false, oldClrType: typeof(long), oldType: "bigint");
            migrationBuilder.AlterColumn<int>("ItemId",       "Item_cs", "int", nullable: false, oldClrType: typeof(long), oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1").OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey("PK_WarehouseTransaction",  "WarehouseTransaction",  "BinId");
            migrationBuilder.AddPrimaryKey("PK_WareHouse_cs",          "WareHouse_cs",          "WarehouseId");
            migrationBuilder.AddPrimaryKey("PK_UOM_cs",                "UOM_cs",                "UOMId");
            migrationBuilder.AddPrimaryKey("PK_StockLedgerTransaction","StockLedgerTransaction","SLTId");
            migrationBuilder.AddPrimaryKey("PK_ItemUOMConversion_cs",  "ItemUOMConversion_cs",  "Id");
            migrationBuilder.AddPrimaryKey("PK_ItemType_s",            "ItemType_s",            "ItemTypeId");
            migrationBuilder.AddPrimaryKey("PK_ItemGroup_cs",          "ItemGroup_cs",          "ItemGroupId");
            migrationBuilder.AddPrimaryKey("PK_Item_cs",               "Item_cs",               "ItemId");

            migrationBuilder.InsertData(
                table: "ItemType_s",
                columns: new[] { "ItemTypeId", "Name" },
                values: new object[,]
                {
                    { 1, "Stock Item" },
                    { 2, "Service" },
                    { 3, "Non-Stock Item" }
                });

            migrationBuilder.CreateIndex("IX_Item_cs_DefaultUOMId",              "Item_cs",               "DefaultUOMId");
            migrationBuilder.CreateIndex("IX_Item_cs_ItemGroupId",               "Item_cs",               "ItemGroupId");
            migrationBuilder.CreateIndex("IX_Item_cs_ItemTypeId",                "Item_cs",               "ItemTypeId");
            migrationBuilder.CreateIndex("IX_ItemGroup_cs_ParentItemGroupId",    "ItemGroup_cs",          "ParentItemGroupId");
            migrationBuilder.CreateIndex("IX_ItemUOMConversion_cs_ItemId",       "ItemUOMConversion_cs",  "ItemId");
            migrationBuilder.CreateIndex("IX_ItemUOMConversion_cs_UOMId",        "ItemUOMConversion_cs",  "UOMId");
            migrationBuilder.CreateIndex("IX_StockLedgerTransaction_ItemId",     "StockLedgerTransaction","ItemId");
            migrationBuilder.CreateIndex("IX_StockLedgerTransaction_WarehouseId","StockLedgerTransaction","WarehouseId");
            migrationBuilder.CreateIndex("IX_WareHouse_cs_ParentWarehouseId",    "WareHouse_cs",          "ParentWarehouseId");
            migrationBuilder.CreateIndex("IX_WarehouseTransaction_WarehouseId",  "WarehouseTransaction",  "WarehouseId");
            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransaction_ItemId_WarehouseId",
                table: "WarehouseTransaction",
                columns: new[] { "ItemId", "WarehouseId" },
                unique: true);

            migrationBuilder.AddForeignKey("FK_Item_cs_ItemGroup_cs_ItemGroupId",               "Item_cs",               "ItemGroupId",       "ItemGroup_cs",          principalColumn: "ItemGroupId",  onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_Item_cs_ItemType_s_ItemTypeId",                  "Item_cs",               "ItemTypeId",        "ItemType_s",            principalColumn: "ItemTypeId",   onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_Item_cs_UOM_cs_DefaultUOMId",                    "Item_cs",               "DefaultUOMId",      "UOM_cs",                principalColumn: "UOMId",        onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_ItemGroup_cs_ItemGroup_cs_ParentItemGroupId",    "ItemGroup_cs",          "ParentItemGroupId", "ItemGroup_cs",          principalColumn: "ItemGroupId",  onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_ItemUOMConversion_cs_Item_cs_ItemId",            "ItemUOMConversion_cs",  "ItemId",            "Item_cs",               principalColumn: "ItemId",       onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_ItemUOMConversion_cs_UOM_cs_UOMId",              "ItemUOMConversion_cs",  "UOMId",             "UOM_cs",                principalColumn: "UOMId",        onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_StockLedgerTransaction_Item_cs_ItemId",          "StockLedgerTransaction","ItemId",            "Item_cs",               principalColumn: "ItemId",       onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_StockLedgerTransaction_WareHouse_cs_WarehouseId","StockLedgerTransaction","WarehouseId",       "WareHouse_cs",          principalColumn: "WarehouseId",  onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_WareHouse_cs_WareHouse_cs_ParentWarehouseId",    "WareHouse_cs",          "ParentWarehouseId", "WareHouse_cs",          principalColumn: "WarehouseId",  onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_WarehouseTransaction_Item_cs_ItemId",            "WarehouseTransaction",  "ItemId",            "Item_cs",               principalColumn: "ItemId",       onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_WarehouseTransaction_WareHouse_cs_WarehouseId",  "WarehouseTransaction",  "WarehouseId",       "WareHouse_cs",          principalColumn: "WarehouseId",  onDelete: ReferentialAction.Restrict);
        }
    }
}
