using Microsoft.EntityFrameworkCore.Migrations;

namespace ReservationSystemDraft_v1.Migrations
{
    public partial class AddActiveAttributeToSittingAndTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "SittingTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Sittings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "SittingTemplates");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Sittings");
        }
    }
}
