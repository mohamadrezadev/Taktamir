using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taktamir.infra.Data.sql.Migrations
{
    public partial class updataorder2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodemeliiCustomer",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodemeliiCustomer",
                table: "Orders");
        }
    }
}
