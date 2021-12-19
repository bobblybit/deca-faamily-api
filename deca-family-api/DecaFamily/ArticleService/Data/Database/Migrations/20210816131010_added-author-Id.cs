using Microsoft.EntityFrameworkCore.Migrations;

namespace ArticleService.Data.Database.Migrations
{
    public partial class addedauthorId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleLike_Articles_ArticleId",
                table: "ArticleLike");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentLike_Comments_CommentId",
                table: "CommentLike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentLike",
                table: "CommentLike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArticleLike",
                table: "ArticleLike");

            migrationBuilder.RenameTable(
                name: "CommentLike",
                newName: "CommentLikes");

            migrationBuilder.RenameTable(
                name: "ArticleLike",
                newName: "ArticleLikes");

            migrationBuilder.RenameIndex(
                name: "IX_CommentLike_CommentId",
                table: "CommentLikes",
                newName: "IX_CommentLikes_CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_ArticleLike_ArticleId",
                table: "ArticleLikes",
                newName: "IX_ArticleLikes_ArticleId");

            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Comments",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentLikes",
                table: "CommentLikes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArticleLikes",
                table: "ArticleLikes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleLikes_Articles_ArticleId",
                table: "ArticleLikes",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentLikes_Comments_CommentId",
                table: "CommentLikes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleLikes_Articles_ArticleId",
                table: "ArticleLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentLikes_Comments_CommentId",
                table: "CommentLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentLikes",
                table: "CommentLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArticleLikes",
                table: "ArticleLikes");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "CommentLikes",
                newName: "CommentLike");

            migrationBuilder.RenameTable(
                name: "ArticleLikes",
                newName: "ArticleLike");

            migrationBuilder.RenameIndex(
                name: "IX_CommentLikes_CommentId",
                table: "CommentLike",
                newName: "IX_CommentLike_CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_ArticleLikes_ArticleId",
                table: "ArticleLike",
                newName: "IX_ArticleLike_ArticleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentLike",
                table: "CommentLike",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArticleLike",
                table: "ArticleLike",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleLike_Articles_ArticleId",
                table: "ArticleLike",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentLike_Comments_CommentId",
                table: "CommentLike",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
