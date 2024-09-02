using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EloBot.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchConfirmationFields2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Player2Confirmed",
                table: "Matches",
                newName: "Player2ConfirmedWin");

            migrationBuilder.RenameColumn(
                name: "Player1Confirmed",
                table: "Matches",
                newName: "Player1ConfirmedWin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Player2ConfirmedWin",
                table: "Matches",
                newName: "Player2Confirmed");

            migrationBuilder.RenameColumn(
                name: "Player1ConfirmedWin",
                table: "Matches",
                newName: "Player1Confirmed");
        }
    }
}
