using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
	/// <inheritdoc />
	public partial class AddConversionGroupToFactor : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "UOMConversionGroupId",
				table: "UOMConversionFactor_cs",
				type: "int",
				nullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_UOMConversionFactor_cs_UOMConversionGroupId",
				table: "UOMConversionFactor_cs",
				column: "UOMConversionGroupId");

			migrationBuilder.AddForeignKey(
				name: "FK_UOMConversionFactor_cs_UOMConversionGroup_cs_UOMConversionGroupId",
				table: "UOMConversionFactor_cs",
				column: "UOMConversionGroupId",
				principalTable: "UOMConversionGroup_cs",
				principalColumn: "Id",
				onDelete: ReferentialAction.SetNull);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_UOMConversionFactor_cs_UOMConversionGroup_cs_UOMConversionGroupId",
				table: "UOMConversionFactor_cs");

			migrationBuilder.DropIndex(
				name: "IX_UOMConversionFactor_cs_UOMConversionGroupId",
				table: "UOMConversionFactor_cs");

			migrationBuilder.DropColumn(
				name: "UOMConversionGroupId",
				table: "UOMConversionFactor_cs");
		}
	}
}
