using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReservationSystemDraft_v1.Migrations
{
    public partial class AddSittingTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultReservationDuration",
                table: "Sittings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SittingTemplateId",
                table: "Sittings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SittingTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    SittingTypeId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Increment = table.Column<int>(type: "int", nullable: false),
                    DefaultReservationDuration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SittingTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SittingTemplates_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SittingTemplates_SittingTypes_SittingTypeId",
                        column: x => x.SittingTypeId,
                        principalTable: "SittingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sittings_SittingTemplateId",
                table: "Sittings",
                column: "SittingTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SittingTemplates_RestaurantId",
                table: "SittingTemplates",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_SittingTemplates_SittingTypeId",
                table: "SittingTemplates",
                column: "SittingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sittings_SittingTemplates_SittingTemplateId",
                table: "Sittings",
                column: "SittingTemplateId",
                principalTable: "SittingTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sittings_SittingTemplates_SittingTemplateId",
                table: "Sittings");

            migrationBuilder.DropTable(
                name: "SittingTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Sittings_SittingTemplateId",
                table: "Sittings");

            migrationBuilder.DropColumn(
                name: "DefaultReservationDuration",
                table: "Sittings");

            migrationBuilder.DropColumn(
                name: "SittingTemplateId",
                table: "Sittings");
        }
    }
}
