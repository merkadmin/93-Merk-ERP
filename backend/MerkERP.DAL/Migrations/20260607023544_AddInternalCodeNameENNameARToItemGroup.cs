using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddInternalCodeNameENNameARToItemGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ItemGroup_cs",
                newName: "Name_EN");

            migrationBuilder.AddColumn<string>(
                name: "InternalCode",
                table: "ItemGroup_cs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name_AR",
                table: "ItemGroup_cs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalCode",
                table: "ItemGroup_cs");

            migrationBuilder.DropColumn(
                name: "Name_AR",
                table: "ItemGroup_cs");

            migrationBuilder.RenameColumn(
                name: "Name_EN",
                table: "ItemGroup_cs",
                newName: "Name");
        }
    }
}
