using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    public partial class AlterSpGetTableMetaDataByEntityKey : Migration
    {
        protected override void Up(MigrationBuilder m)
        {
            // isMain and isParent are neutral boolean flags, not active/inactive status
            m.UpdateData("TableMetaData", "Id", 12L, "RenderAs", "yesno");
            m.UpdateData("TableMetaData", "Id", 34L, "RenderAs", "yesno");
        }

        protected override void Down(MigrationBuilder m)
        {
            m.UpdateData("TableMetaData", "Id", 12L, "RenderAs", "badge");
            m.UpdateData("TableMetaData", "Id", 34L, "RenderAs", "badge");
        }
    }
}
