using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taktamir.infra.Data.sql.Migrations
{
    public partial class migrationsqlserver2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_AspNetUsers_Adminid",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_Adminid",
                table: "Rooms");

            migrationBuilder.AddColumn<int>(
                name: "Roomid",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Roomid",
                table: "AspNetUsers",
                column: "Roomid",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Rooms_Roomid",
                table: "AspNetUsers",
                column: "Roomid",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Rooms_Roomid",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Roomid",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Roomid",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Adminid",
                table: "Rooms",
                column: "Adminid",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_AspNetUsers_Adminid",
                table: "Rooms",
                column: "Adminid",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
