using Microsoft.EntityFrameworkCore.Migrations;

namespace AccountManagement.Data.Database.Migrations
{
    public partial class AddLinkToSocialHandle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "SocialHandles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Link",
                table: "SocialHandles");
        }
    }
}
