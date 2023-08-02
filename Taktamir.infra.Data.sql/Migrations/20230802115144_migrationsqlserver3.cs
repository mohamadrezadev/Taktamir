using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taktamir.infra.Data.sql.Migrations
{
    public partial class migrationsqlserver3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Rooms_Roomid",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Roomid",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Adminid",
                table: "Rooms",
                newName: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_UserId",
                table: "Rooms",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_AspNetUsers_UserId",
                table: "Rooms",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_AspNetUsers_UserId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_UserId",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Rooms",
                newName: "Adminid");

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
    }
}
