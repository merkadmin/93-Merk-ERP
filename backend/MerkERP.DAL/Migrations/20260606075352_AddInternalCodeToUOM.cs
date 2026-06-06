using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddInternalCodeToUOM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InternalCode",
                table: "UOM_cs",
                type: "nvarchar(50)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalCode",
                table: "UOM_cs");
        }
    }
}
