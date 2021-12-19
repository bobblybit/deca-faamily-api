using Microsoft.EntityFrameworkCore.Migrations;

namespace AccountManagement.Data.Database.Migrations
{
    public partial class AddPositionToDepartment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Department",
                maxLength: 125,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Department");
        }
    }
}
