using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taktamir.infra.Data.sql.Migrations
{
    public partial class configorders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Orders_orderid",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_orderid",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "orderid",
                table: "Jobs");

            migrationBuilder.CreateTable(
                name: "OrderJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderJobs_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderJobs_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderJobs_JobId",
                table: "OrderJobs",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderJobs_OrderId",
                table: "OrderJobs",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderJobs");

            migrationBuilder.AddColumn<int>(
                name: "orderid",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_orderid",
                table: "Jobs",
                column: "orderid");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Orders_orderid",
                table: "Jobs",
                column: "orderid",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}
