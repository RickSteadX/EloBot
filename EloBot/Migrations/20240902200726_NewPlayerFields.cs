using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EloBot.Migrations
{
    /// <inheritdoc />
    public partial class NewPlayerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SecondPlayerId",
                table: "Matches",
                newName: "SecondPlayerDiscordId");

            migrationBuilder.RenameColumn(
                name: "FirstPlayerId",
                table: "Matches",
                newName: "FirstPlayerDiscordId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_FirstPlayerDiscordId",
                table: "Matches",
                column: "FirstPlayerDiscordId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_SecondPlayerDiscordId",
                table: "Matches",
                column: "SecondPlayerDiscordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Players_FirstPlayerDiscordId",
                table: "Matches",
                column: "FirstPlayerDiscordId",
                principalTable: "Players",
                principalColumn: "DiscordId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Players_SecondPlayerDiscordId",
                table: "Matches",
                column: "SecondPlayerDiscordId",
                principalTable: "Players",
                principalColumn: "DiscordId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Players_FirstPlayerDiscordId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Players_SecondPlayerDiscordId",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_FirstPlayerDiscordId",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_SecondPlayerDiscordId",
                table: "Matches");

            migrationBuilder.RenameColumn(
                name: "SecondPlayerDiscordId",
                table: "Matches",
                newName: "SecondPlayerId");

            migrationBuilder.RenameColumn(
                name: "FirstPlayerDiscordId",
                table: "Matches",
                newName: "FirstPlayerId");
        }
    }
}
