using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
	/// <inheritdoc />
	public partial class RenameUOMColumns : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Name",
				table: "UOM_cs");

			migrationBuilder.RenameColumn(
				name: "UOMId",
				table: "UOM_cs",
				newName: "Id");

			migrationBuilder.AddColumn<string>(
				name: "Name_AR",
				table: "UOM_cs",
				type: "nvarchar(200)",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Name_EN",
				table: "UOM_cs",
				type: "nvarchar(200)",
				nullable: false,
				defaultValue: "");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Name_AR",
				table: "UOM_cs");

			migrationBuilder.DropColumn(
				name: "Name_EN",
				table: "UOM_cs");

			migrationBuilder.RenameColumn(
				name: "Id",
				table: "UOM_cs",
				newName: "UOMId");

			migrationBuilder.AddColumn<string>(
				name: "Name",
				table: "UOM_cs",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "");
		}
	}
}
