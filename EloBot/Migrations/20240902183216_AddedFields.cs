using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EloBot.Migrations
{
    /// <inheritdoc />
    public partial class AddedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Player1ConfirmedWin",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Player1MessageId",
                table: "Matches");

            migrationBuilder.RenameColumn(
                name: "Player2MessageId",
                table: "Matches",
                newName: "SecondPlayerConfirmedWin");

            migrationBuilder.RenameColumn(
                name: "Player2Id",
                table: "Matches",
                newName: "SecondPlayerId");

            migrationBuilder.RenameColumn(
                name: "Player2ConfirmedWin",
                table: "Matches",
                newName: "FirstPlayerConfirmedWin");

            migrationBuilder.RenameColumn(
                name: "Player1Id",
                table: "Matches",
                newName: "SecondMessageId");

            migrationBuilder.AddColumn<ulong>(
                name: "FirstMessageChannelId",
                table: "Matches",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "FirstMessageId",
                table: "Matches",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "FirstPlayerId",
                table: "Matches",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "SecondMessageChannelId",
                table: "Matches",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstMessageChannelId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "FirstMessageId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "FirstPlayerId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "SecondMessageChannelId",
                table: "Matches");

            migrationBuilder.RenameColumn(
                name: "SecondPlayerId",
                table: "Matches",
                newName: "Player2Id");

            migrationBuilder.RenameColumn(
                name: "SecondPlayerConfirmedWin",
                table: "Matches",
                newName: "Player2MessageId");

            migrationBuilder.RenameColumn(
                name: "SecondMessageId",
                table: "Matches",
                newName: "Player1Id");

            migrationBuilder.RenameColumn(
                name: "FirstPlayerConfirmedWin",
                table: "Matches",
                newName: "Player2ConfirmedWin");

            migrationBuilder.AddColumn<bool>(
                name: "Player1ConfirmedWin",
                table: "Matches",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "Player1MessageId",
                table: "Matches",
                type: "INTEGER",
                nullable: true);
        }
    }
}
