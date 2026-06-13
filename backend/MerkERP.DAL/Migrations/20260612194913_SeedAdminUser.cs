using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "User_cs",
                columns: new[] { "Id", "Description", "Email", "Login", "Name_AR", "Name_EN", "Password", "UserTypeId" },
                values: new object[] { 1L, null, null, "admin", "مدير", "Admin", "admin", 1L });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User_cs",
                keyColumn: "Id",
                keyValue: 1L);
        }
    }
}
