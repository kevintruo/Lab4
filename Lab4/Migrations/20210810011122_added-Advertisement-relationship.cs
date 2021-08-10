using Microsoft.EntityFrameworkCore.Migrations;

namespace Lab4.Migrations
{
    public partial class addedAdvertisementrelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommunityId",
                table: "Advertisement",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Advertisement_CommunityId",
                table: "Advertisement",
                column: "CommunityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advertisement_Community_CommunityId",
                table: "Advertisement",
                column: "CommunityId",
                principalTable: "Community",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advertisement_Community_CommunityId",
                table: "Advertisement");

            migrationBuilder.DropIndex(
                name: "IX_Advertisement_CommunityId",
                table: "Advertisement");

            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "Advertisement");
        }
    }
}
