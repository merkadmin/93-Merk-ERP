using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditColumnsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InsertedBy",
                table: "User_cs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedDate",
                table: "User_cs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "User_cs",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData(
                table: "User_cs",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "InsertedBy", "InsertedDate", "IsActive" },
                values: new object[] { null, null, true });

            migrationBuilder.CreateIndex(
                name: "IX_User_cs_InsertedBy",
                table: "User_cs",
                column: "InsertedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_User_cs_User_cs_InsertedBy",
                table: "User_cs",
                column: "InsertedBy",
                principalTable: "User_cs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_cs_User_cs_InsertedBy",
                table: "User_cs");

            migrationBuilder.DropIndex(
                name: "IX_User_cs_InsertedBy",
                table: "User_cs");

            migrationBuilder.DropColumn(name: "InsertedBy",   table: "User_cs");
            migrationBuilder.DropColumn(name: "InsertedDate", table: "User_cs");
            migrationBuilder.DropColumn(name: "IsActive",     table: "User_cs");
        }
    }
}
