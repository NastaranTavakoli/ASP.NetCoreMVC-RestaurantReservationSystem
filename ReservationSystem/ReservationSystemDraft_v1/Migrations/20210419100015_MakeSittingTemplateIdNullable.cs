using Microsoft.EntityFrameworkCore.Migrations;

namespace ReservationSystemDraft_v1.Migrations
{
    public partial class MakeSittingTemplateIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sittings_SittingTemplates_SittingTemplateId",
                table: "Sittings");

            migrationBuilder.AlterColumn<int>(
                name: "SittingTemplateId",
                table: "Sittings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Sittings_SittingTemplates_SittingTemplateId",
                table: "Sittings",
                column: "SittingTemplateId",
                principalTable: "SittingTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sittings_SittingTemplates_SittingTemplateId",
                table: "Sittings");

            migrationBuilder.AlterColumn<int>(
                name: "SittingTemplateId",
                table: "Sittings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sittings_SittingTemplates_SittingTemplateId",
                table: "Sittings",
                column: "SittingTemplateId",
                principalTable: "SittingTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
