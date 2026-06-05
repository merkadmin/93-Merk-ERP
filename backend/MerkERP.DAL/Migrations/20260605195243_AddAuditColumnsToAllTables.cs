using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditColumnsToAllTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InsertedBy",
                table: "WarehouseTransaction",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedDate",
                table: "WarehouseTransaction",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "WarehouseTransaction",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "WarehouseTransaction",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "InsertedBy",
                table: "WareHouse_cs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedDate",
                table: "WareHouse_cs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "WareHouse_cs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "InsertedBy",
                table: "UOMConversionGroup_cs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedDate",
                table: "UOMConversionGroup_cs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UOMConversionGroup_cs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "UOMConversionGroup_cs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "InsertedBy",
                table: "UOMConversionFactor_cs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedDate",
                table: "UOMConversionFactor_cs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UOMConversionFactor_cs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "UOMConversionFactor_cs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "InsertedBy",
                table: "StockLedgerTransaction",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedDate",
                table: "StockLedgerTransaction",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "StockLedgerTransaction",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "StockLedgerTransaction",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "InsertedBy",
                table: "ItemUOMConversion_cs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedDate",
                table: "ItemUOMConversion_cs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ItemUOMConversion_cs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "ItemUOMConversion_cs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "InsertedBy",
                table: "ItemType_s",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedDate",
                table: "ItemType_s",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ItemType_s",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "ItemType_s",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "InsertedBy",
                table: "ItemGroup_cs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedDate",
                table: "ItemGroup_cs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "ItemGroup_cs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "InsertedBy",
                table: "Item_cs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedDate",
                table: "Item_cs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Item_cs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ItemType_s",
                keyColumn: "ItemTypeId",
                keyValue: 1L,
                columns: new[] { "InsertedBy", "InsertedDate", "IsActive", "IsFavorite" },
                values: new object[] { null, null, true, false });

            migrationBuilder.UpdateData(
                table: "ItemType_s",
                keyColumn: "ItemTypeId",
                keyValue: 2L,
                columns: new[] { "InsertedBy", "InsertedDate", "IsActive", "IsFavorite" },
                values: new object[] { null, null, true, false });

            migrationBuilder.UpdateData(
                table: "ItemType_s",
                keyColumn: "ItemTypeId",
                keyValue: 3L,
                columns: new[] { "InsertedBy", "InsertedDate", "IsActive", "IsFavorite" },
                values: new object[] { null, null, true, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "WarehouseTransaction");

            migrationBuilder.DropColumn(
                name: "InsertedDate",
                table: "WarehouseTransaction");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "WarehouseTransaction");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "WarehouseTransaction");

            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "WareHouse_cs");

            migrationBuilder.DropColumn(
                name: "InsertedDate",
                table: "WareHouse_cs");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "WareHouse_cs");

            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "UOMConversionGroup_cs");

            migrationBuilder.DropColumn(
                name: "InsertedDate",
                table: "UOMConversionGroup_cs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UOMConversionGroup_cs");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "UOMConversionGroup_cs");

            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "UOMConversionFactor_cs");

            migrationBuilder.DropColumn(
                name: "InsertedDate",
                table: "UOMConversionFactor_cs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UOMConversionFactor_cs");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "UOMConversionFactor_cs");

            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "StockLedgerTransaction");

            migrationBuilder.DropColumn(
                name: "InsertedDate",
                table: "StockLedgerTransaction");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "StockLedgerTransaction");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "StockLedgerTransaction");

            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "ItemUOMConversion_cs");

            migrationBuilder.DropColumn(
                name: "InsertedDate",
                table: "ItemUOMConversion_cs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ItemUOMConversion_cs");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "ItemUOMConversion_cs");

            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "ItemType_s");

            migrationBuilder.DropColumn(
                name: "InsertedDate",
                table: "ItemType_s");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ItemType_s");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "ItemType_s");

            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "ItemGroup_cs");

            migrationBuilder.DropColumn(
                name: "InsertedDate",
                table: "ItemGroup_cs");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "ItemGroup_cs");

            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "Item_cs");

            migrationBuilder.DropColumn(
                name: "InsertedDate",
                table: "Item_cs");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Item_cs");
        }
    }
}
