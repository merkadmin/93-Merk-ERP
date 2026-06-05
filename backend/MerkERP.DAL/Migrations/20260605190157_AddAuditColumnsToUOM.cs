using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
	/// <inheritdoc />
	public partial class AddAuditColumnsToUOM : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<long>(
				name: "InsertedBy",
				table: "UOM_cs",
				type: "bigint",
				nullable: true);

			migrationBuilder.AddColumn<DateTime>(
				name: "InsertedDate",
				table: "UOM_cs",
				type: "datetime2",
				nullable: true);

			migrationBuilder.AddColumn<bool>(
				name: "IsActive",
				table: "UOM_cs",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				name: "IsFavorite",
				table: "UOM_cs",
				type: "bit",
				nullable: false,
				defaultValue: false);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "InsertedBy",
				table: "UOM_cs");

			migrationBuilder.DropColumn(
				name: "InsertedDate",
				table: "UOM_cs");

			migrationBuilder.DropColumn(
				name: "IsActive",
				table: "UOM_cs");

			migrationBuilder.DropColumn(
				name: "IsFavorite",
				table: "UOM_cs");
		}
	}
}
