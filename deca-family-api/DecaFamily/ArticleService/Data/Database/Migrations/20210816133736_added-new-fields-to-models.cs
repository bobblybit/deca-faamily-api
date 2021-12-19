using Microsoft.EntityFrameworkCore.Migrations;

namespace ArticleService.Data.Database.Migrations
{
    public partial class addednewfieldstomodels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LikerId",
                table: "CommentLikes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Articles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Stack",
                table: "Articles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StackId",
                table: "Articles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LikerId",
                table: "ArticleLikes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LikerId",
                table: "CommentLikes");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Stack",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "StackId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "LikerId",
                table: "ArticleLikes");
        }
    }
}
