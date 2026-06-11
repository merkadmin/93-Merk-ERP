using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddWareHouseType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WareHouseType_s",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name_EN = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Name_AR = table.Column<string>(type: "nvarchar(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WareHouseType_s", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "WareHouseType_s",
                columns: new[] { "Id", "Name_AR", "Name_EN" },
                values: new object[,]
                {
                    { 1L, "مستودعات الأدوية", "Pharmaceutical Warehouses" },
                    { 2L, "مستودعات المستهلكات ومعدات الوقاية الشخصية", "Consumables & PPE Warehouses" },
                    { 3L, "مستودعات الأجهزة والمعدات الطبية", "Medical Device & Equipment Warehouses" },
                    { 4L, "مستودعات العمليات الجراحية المعقمة", "Sterile Surgical Warehouses" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WareHouseType_s");
        }
    }
}
