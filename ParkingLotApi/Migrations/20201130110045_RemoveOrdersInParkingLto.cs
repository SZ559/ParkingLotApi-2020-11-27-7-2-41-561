using Microsoft.EntityFrameworkCore.Migrations;

namespace ParkingLotApi.Migrations
{
    public partial class RemoveOrdersInParkingLto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ParkingLots_ParkingLotEntityId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ParkingLotEntityId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ParkingLotEntityId",
                table: "Orders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParkingLotEntityId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ParkingLotEntityId",
                table: "Orders",
                column: "ParkingLotEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ParkingLots_ParkingLotEntityId",
                table: "Orders",
                column: "ParkingLotEntityId",
                principalTable: "ParkingLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
