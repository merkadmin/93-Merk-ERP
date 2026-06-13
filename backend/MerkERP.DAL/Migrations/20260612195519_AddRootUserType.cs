using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRootUserType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Use raw SQL so this migration is safe to re-run if the data was already set manually
            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT UserType_s ON;
                IF NOT EXISTS (SELECT 1 FROM UserType_s WHERE Id = 4)
                    INSERT INTO UserType_s (Id, Name_EN, Name_AR) VALUES (4, 'Regular User', N'مستخدم عادي');
                SET IDENTITY_INSERT UserType_s OFF;

                UPDATE UserType_s SET Name_EN = 'Root',       Name_AR = N'روت'        WHERE Id = 1;
                UPDATE UserType_s SET Name_EN = 'Merk Admin', Name_AR = N'مدير ميرك'  WHERE Id = 2;
                UPDATE UserType_s SET Name_EN = 'Admin',      Name_AR = N'مدير'       WHERE Id = 3;

                UPDATE User_cs SET UserTypeId = 2 WHERE Id = 1 AND UserTypeId = 1;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE User_cs SET UserTypeId = 1 WHERE Id = 1 AND UserTypeId = 2;

                UPDATE UserType_s SET Name_EN = 'Merk Admin',   Name_AR = N'مدير ميرك'   WHERE Id = 1;
                UPDATE UserType_s SET Name_EN = 'Admin',        Name_AR = N'مدير'         WHERE Id = 2;
                UPDATE UserType_s SET Name_EN = 'Regular User', Name_AR = N'مستخدم عادي' WHERE Id = 3;

                DELETE FROM UserType_s WHERE Id = 4;
            ");
        }
    }
}
