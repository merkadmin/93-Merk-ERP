using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameUOMConversionGroupToFactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop FKs and indexes before renaming
            migrationBuilder.DropForeignKey("FK_UOMConversionGroup_cs_UOM_cs_UOMFromId", "UOMConversionGroup_cs");
            migrationBuilder.DropForeignKey("FK_UOMConversionGroup_cs_UOM_cs_UOMToId",   "UOMConversionGroup_cs");
            migrationBuilder.DropIndex("IX_UOMConversionGroup_cs_UOMFromId", "UOMConversionGroup_cs");
            migrationBuilder.DropIndex("IX_UOMConversionGroup_cs_UOMToId",   "UOMConversionGroup_cs");
            migrationBuilder.DropPrimaryKey("PK_UOMConversionGroup_cs", "UOMConversionGroup_cs");

            // Rename the table
            migrationBuilder.RenameTable(name: "UOMConversionGroup_cs", newName: "UOMConversionFactor_cs");

            // Recreate PK, indexes, and FKs with the new table name
            migrationBuilder.AddPrimaryKey("PK_UOMConversionFactor_cs", "UOMConversionFactor_cs", "Id");
            migrationBuilder.CreateIndex("IX_UOMConversionFactor_cs_UOMFromId", "UOMConversionFactor_cs", "UOMFromId");
            migrationBuilder.CreateIndex("IX_UOMConversionFactor_cs_UOMToId",   "UOMConversionFactor_cs", "UOMToId");
            migrationBuilder.AddForeignKey("FK_UOMConversionFactor_cs_UOM_cs_UOMFromId", "UOMConversionFactor_cs", "UOMFromId", "UOM_cs", principalColumn: "UOMId", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_UOMConversionFactor_cs_UOM_cs_UOMToId",   "UOMConversionFactor_cs", "UOMToId",   "UOM_cs", principalColumn: "UOMId", onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_UOMConversionFactor_cs_UOM_cs_UOMFromId", "UOMConversionFactor_cs");
            migrationBuilder.DropForeignKey("FK_UOMConversionFactor_cs_UOM_cs_UOMToId",   "UOMConversionFactor_cs");
            migrationBuilder.DropIndex("IX_UOMConversionFactor_cs_UOMFromId", "UOMConversionFactor_cs");
            migrationBuilder.DropIndex("IX_UOMConversionFactor_cs_UOMToId",   "UOMConversionFactor_cs");
            migrationBuilder.DropPrimaryKey("PK_UOMConversionFactor_cs", "UOMConversionFactor_cs");

            migrationBuilder.RenameTable(name: "UOMConversionFactor_cs", newName: "UOMConversionGroup_cs");

            migrationBuilder.AddPrimaryKey("PK_UOMConversionGroup_cs", "UOMConversionGroup_cs", "Id");
            migrationBuilder.CreateIndex("IX_UOMConversionGroup_cs_UOMFromId", "UOMConversionGroup_cs", "UOMFromId");
            migrationBuilder.CreateIndex("IX_UOMConversionGroup_cs_UOMToId",   "UOMConversionGroup_cs", "UOMToId");
            migrationBuilder.AddForeignKey("FK_UOMConversionGroup_cs_UOM_cs_UOMFromId", "UOMConversionGroup_cs", "UOMFromId", "UOM_cs", principalColumn: "UOMId", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_UOMConversionGroup_cs_UOM_cs_UOMToId",   "UOMConversionGroup_cs", "UOMToId",   "UOM_cs", principalColumn: "UOMId", onDelete: ReferentialAction.Restrict);
        }
    }
}
