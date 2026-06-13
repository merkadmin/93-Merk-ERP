using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAndUserType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserType_s",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name_EN = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Name_AR = table.Column<string>(type: "nvarchar(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserType_s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User_cs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name_EN = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Name_AR = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    Login = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    UserTypeId = table.Column<long>(type: "bigint", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_cs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_cs_UserType_s_UserTypeId",
                        column: x => x.UserTypeId,
                        principalTable: "UserType_s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 32L,
                column: "ColumnOrder",
                value: 6);

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 33L,
                column: "ColumnOrder",
                value: 7);

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 34L,
                column: "ColumnOrder",
                value: 4);

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 35L,
                column: "ColumnOrder",
                value: 8);

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 39L,
                column: "ColumnOrder",
                value: 5);

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 40L,
                column: "ColumnOrder",
                value: 6);

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 45L,
                column: "ColumnOrder",
                value: 4);

            migrationBuilder.InsertData(
                table: "TableMetaData",
                columns: new[] { "Id", "ColumnOrder", "DataType", "EntityProperty", "FilterType", "ForeignKeyProperty", "IsFilterable", "IsSortable", "IsVisible", "Key", "LabelAR", "LabelEN", "MinWidth", "RenderAs", "TableNameId" },
                values: new object[] { 46L, 5, "string", "ParentWareHouse", "select", "ParentWareHouse", true, true, true, "parentWareHouse", "الرئيسي", "Parent", null, "text", 14 });

            migrationBuilder.InsertData(
                table: "TableName_s",
                columns: new[] { "Id", "EntityKey", "Name" },
                values: new object[,]
                {
                    { 19, null, "UserType_s" },
                    { 20, null, "User_cs" }
                });

            migrationBuilder.InsertData(
                table: "UserType_s",
                columns: new[] { "Id", "Name_AR", "Name_EN" },
                values: new object[,]
                {
                    { 1L, "مدير ميرك", "Merk Admin" },
                    { 2L, "مدير", "Admin" },
                    { 3L, "مستخدم عادي", "Regular User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransaction_InsertedBy",
                table: "WarehouseTransaction",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WareHouseCategory_cs_InsertedBy",
                table: "WareHouseCategory_cs",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WareHouse_cs_InsertedBy",
                table: "WareHouse_cs",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UOMConversionGroup_cs_InsertedBy",
                table: "UOMConversionGroup_cs",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UOMConversionFactor_cs_InsertedBy",
                table: "UOMConversionFactor_cs",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UOM_cs_InsertedBy",
                table: "UOM_cs",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgerTransaction_InsertedBy",
                table: "StockLedgerTransaction",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ItemUOMConversion_cs_InsertedBy",
                table: "ItemUOMConversion_cs",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ItemType_s_InsertedBy",
                table: "ItemType_s",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ItemGroup_cs_InsertedBy",
                table: "ItemGroup_cs",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Item_cs_InsertedBy",
                table: "Item_cs",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Branch_cs_InsertedBy",
                table: "Branch_cs",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_User_cs_UserTypeId",
                table: "User_cs",
                column: "UserTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_cs_User_cs_InsertedBy",
                table: "Branch_cs",
                column: "InsertedBy",
                principalTable: "User_cs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Item_cs_User_cs_InsertedBy",
                table: "Item_cs",
                column: "InsertedBy",
                principalTable: "User_cs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemGroup_cs_User_cs_InsertedBy",
                table: "ItemGroup_cs",
                column: "InsertedBy",
                principalTable: "User_cs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemType_s_User_cs_InsertedBy",
                table: "ItemType_s",
                column: "InsertedBy",
                principalTable: "User_cs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemUOMConversion_cs_User_cs_InsertedBy",
                table: "ItemUOMConversion_cs",
                column: "InsertedBy",
                principalTable: "User_cs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLedgerTransaction_User_cs_InsertedBy",
                table: "StockLedgerTransaction",
                column: "InsertedBy",
                principalTable: "User_cs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UOM_cs_User_cs_InsertedBy",
                table: "UOM_cs",
                column: "InsertedBy",
                principalTable: "User_cs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UOMConversionFactor_cs_User_cs_InsertedBy",
                table: "UOMConversionFactor_cs",
                column: "InsertedBy",
                principalTable: "User_cs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UOMConversionGroup_cs_User_cs_InsertedBy",
                table: "UOMConversionGroup_cs",
                column: "InsertedBy",
                principalTable: "User_cs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WareHouse_cs_User_cs_InsertedBy",
                table: "WareHouse_cs",
                column: "InsertedBy",
                principalTable: "User_cs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WareHouseCategory_cs_User_cs_InsertedBy",
                table: "WareHouseCategory_cs",
                column: "InsertedBy",
                principalTable: "User_cs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseTransaction_User_cs_InsertedBy",
                table: "WarehouseTransaction",
                column: "InsertedBy",
                principalTable: "User_cs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branch_cs_User_cs_InsertedBy",
                table: "Branch_cs");

            migrationBuilder.DropForeignKey(
                name: "FK_Item_cs_User_cs_InsertedBy",
                table: "Item_cs");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemGroup_cs_User_cs_InsertedBy",
                table: "ItemGroup_cs");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemType_s_User_cs_InsertedBy",
                table: "ItemType_s");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemUOMConversion_cs_User_cs_InsertedBy",
                table: "ItemUOMConversion_cs");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLedgerTransaction_User_cs_InsertedBy",
                table: "StockLedgerTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_UOM_cs_User_cs_InsertedBy",
                table: "UOM_cs");

            migrationBuilder.DropForeignKey(
                name: "FK_UOMConversionFactor_cs_User_cs_InsertedBy",
                table: "UOMConversionFactor_cs");

            migrationBuilder.DropForeignKey(
                name: "FK_UOMConversionGroup_cs_User_cs_InsertedBy",
                table: "UOMConversionGroup_cs");

            migrationBuilder.DropForeignKey(
                name: "FK_WareHouse_cs_User_cs_InsertedBy",
                table: "WareHouse_cs");

            migrationBuilder.DropForeignKey(
                name: "FK_WareHouseCategory_cs_User_cs_InsertedBy",
                table: "WareHouseCategory_cs");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseTransaction_User_cs_InsertedBy",
                table: "WarehouseTransaction");

            migrationBuilder.DropTable(
                name: "User_cs");

            migrationBuilder.DropTable(
                name: "UserType_s");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseTransaction_InsertedBy",
                table: "WarehouseTransaction");

            migrationBuilder.DropIndex(
                name: "IX_WareHouseCategory_cs_InsertedBy",
                table: "WareHouseCategory_cs");

            migrationBuilder.DropIndex(
                name: "IX_WareHouse_cs_InsertedBy",
                table: "WareHouse_cs");

            migrationBuilder.DropIndex(
                name: "IX_UOMConversionGroup_cs_InsertedBy",
                table: "UOMConversionGroup_cs");

            migrationBuilder.DropIndex(
                name: "IX_UOMConversionFactor_cs_InsertedBy",
                table: "UOMConversionFactor_cs");

            migrationBuilder.DropIndex(
                name: "IX_UOM_cs_InsertedBy",
                table: "UOM_cs");

            migrationBuilder.DropIndex(
                name: "IX_StockLedgerTransaction_InsertedBy",
                table: "StockLedgerTransaction");

            migrationBuilder.DropIndex(
                name: "IX_ItemUOMConversion_cs_InsertedBy",
                table: "ItemUOMConversion_cs");

            migrationBuilder.DropIndex(
                name: "IX_ItemType_s_InsertedBy",
                table: "ItemType_s");

            migrationBuilder.DropIndex(
                name: "IX_ItemGroup_cs_InsertedBy",
                table: "ItemGroup_cs");

            migrationBuilder.DropIndex(
                name: "IX_Item_cs_InsertedBy",
                table: "Item_cs");

            migrationBuilder.DropIndex(
                name: "IX_Branch_cs_InsertedBy",
                table: "Branch_cs");

            migrationBuilder.DeleteData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 46L);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 32L,
                column: "ColumnOrder",
                value: 4);

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 33L,
                column: "ColumnOrder",
                value: 5);

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 34L,
                column: "ColumnOrder",
                value: 6);

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 35L,
                column: "ColumnOrder",
                value: 7);

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 39L,
                column: "ColumnOrder",
                value: 4);

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 40L,
                column: "ColumnOrder",
                value: 5);

            migrationBuilder.UpdateData(
                table: "TableMetaData",
                keyColumn: "Id",
                keyValue: 45L,
                column: "ColumnOrder",
                value: 6);
        }
    }
}
