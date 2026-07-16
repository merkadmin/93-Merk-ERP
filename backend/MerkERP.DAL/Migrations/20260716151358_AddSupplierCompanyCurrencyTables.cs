using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierCompanyCurrencyTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // NOTE: This migration was hand-trimmed. `dotnet ef migrations add` also scaffolded
            // a large set of TableMetaData/User_cs changes (DataType/FilterType/RenderAs -> lookup
            // FK columns, User_cs audit columns) because the committed model snapshot predates
            // migration 20260614120000_AddTableMetaDataLookups, which was applied to the real
            // database via raw SQL outside of `dotnet ef database update` and never updated the
            // snapshot. Those columns/tables already exist in MerkERPDB (verified via
            // INFORMATION_SCHEMA/sys.foreign_keys/sys.indexes), so re-running them here would fail
            // with "already exists" errors. Only the genuinely new Supplier/Company/Currency
            // objects are included below.
            migrationBuilder.CreateTable(
                name: "Currency_s",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(3)", nullable: false),
                    Name_EN = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Name_AR = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Symbol = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency_s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupplierType_s",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name_EN = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Name_AR = table.Column<string>(type: "nvarchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierType_s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Company_cs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternalCode = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Name_EN = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Name_AR = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    Abbr = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    DefaultCurrencyId = table.Column<long>(type: "bigint", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false),
                    InsertedBy = table.Column<long>(type: "bigint", nullable: true),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company_cs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Company_cs_Currency_s_DefaultCurrencyId",
                        column: x => x.DefaultCurrencyId,
                        principalTable: "Currency_s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Company_cs_User_cs_InsertedBy",
                        column: x => x.InsertedBy,
                        principalTable: "User_cs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyExchangeRate_cs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromCurrencyId = table.Column<long>(type: "bigint", nullable: false),
                    ToCurrencyId = table.Column<long>(type: "bigint", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ForBuying = table.Column<bool>(type: "bit", nullable: false),
                    ForSelling = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    InsertedBy = table.Column<long>(type: "bigint", nullable: true),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchangeRate_cs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRate_cs_Currency_s_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "Currency_s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRate_cs_Currency_s_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "Currency_s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRate_cs_User_cs_InsertedBy",
                        column: x => x.InsertedBy,
                        principalTable: "User_cs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Supplier_cs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternalCode = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Name_EN = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Name_AR = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    SupplierTypeId = table.Column<long>(type: "bigint", nullable: true),
                    DefaultCurrencyId = table.Column<long>(type: "bigint", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPersonName = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    ContactMobile = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    IsOnHold = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false),
                    InsertedBy = table.Column<long>(type: "bigint", nullable: true),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier_cs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Supplier_cs_Currency_s_DefaultCurrencyId",
                        column: x => x.DefaultCurrencyId,
                        principalTable: "Currency_s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Supplier_cs_SupplierType_s_SupplierTypeId",
                        column: x => x.SupplierTypeId,
                        principalTable: "SupplierType_s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Supplier_cs_User_cs_InsertedBy",
                        column: x => x.InsertedBy,
                        principalTable: "User_cs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Currency_s",
                columns: new[] { "Id", "Code", "IsActive", "Name_AR", "Name_EN", "Symbol" },
                values: new object[,]
                {
                    { 1L, "EGP", true, "جنيه مصري", "Egyptian Pound", "ج.م" },
                    { 2L, "USD", true, "دولار أمريكي", "US Dollar", "$" },
                    { 3L, "EUR", true, "يورو", "Euro", "€" }
                });

            migrationBuilder.InsertData(
                table: "SupplierType_s",
                columns: new[] { "Id", "Name_AR", "Name_EN" },
                values: new object[,]
                {
                    { 1L, "شركة", "Company" },
                    { 2L, "فرد", "Individual" },
                    { 3L, "شراكة", "Partnership" }
                });

            migrationBuilder.InsertData(
                table: "TableName_s",
                columns: new[] { "Id", "EntityKey", "Name" },
                values: new object[,]
                {
                    { 30, "currencies", "Currency_s" },
                    { 31, null, "SupplierType_s" },
                    { 32, "suppliers", "Supplier_cs" },
                    { 33, "companies", "Company_cs" },
                    { 34, "currency-exchange-rates", "CurrencyExchangeRate_cs" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Company_cs_DefaultCurrencyId",
                table: "Company_cs",
                column: "DefaultCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_cs_InsertedBy",
                table: "Company_cs",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Company_cs_InternalCode",
                table: "Company_cs",
                column: "InternalCode",
                unique: true,
                filter: "[InternalCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Company_cs_Name_EN",
                table: "Company_cs",
                column: "Name_EN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currency_s_Code",
                table: "Currency_s",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRate_cs_FromCurrencyId_ToCurrencyId_EffectiveDate",
                table: "CurrencyExchangeRate_cs",
                columns: new[] { "FromCurrencyId", "ToCurrencyId", "EffectiveDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRate_cs_InsertedBy",
                table: "CurrencyExchangeRate_cs",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRate_cs_ToCurrencyId",
                table: "CurrencyExchangeRate_cs",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_cs_DefaultCurrencyId",
                table: "Supplier_cs",
                column: "DefaultCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_cs_InsertedBy",
                table: "Supplier_cs",
                column: "InsertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_cs_InternalCode",
                table: "Supplier_cs",
                column: "InternalCode",
                unique: true,
                filter: "[InternalCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_cs_SupplierTypeId",
                table: "Supplier_cs",
                column: "SupplierTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Company_cs");

            migrationBuilder.DropTable(
                name: "CurrencyExchangeRate_cs");

            migrationBuilder.DropTable(
                name: "Supplier_cs");

            migrationBuilder.DropTable(
                name: "Currency_s");

            migrationBuilder.DropTable(
                name: "SupplierType_s");

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "TableName_s",
                keyColumn: "Id",
                keyValue: 34);
        }
    }
}
