using Microsoft.EntityFrameworkCore.Migrations;

namespace ReservationSystemDraft_v1.Migrations
{
    public partial class MakeSittingTypesBelongToRestaurant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RestaurantId",
                table: "SittingTypes",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_SittingTypes_RestaurantId",
                table: "SittingTypes",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_SittingTypes_Restaurants_RestaurantId",
                table: "SittingTypes",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SittingTypes_Restaurants_RestaurantId",
                table: "SittingTypes");

            migrationBuilder.DropIndex(
                name: "IX_SittingTypes_RestaurantId",
                table: "SittingTypes");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "SittingTypes");
        }
    }
}
