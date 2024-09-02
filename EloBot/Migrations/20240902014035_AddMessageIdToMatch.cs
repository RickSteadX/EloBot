using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EloBot.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageIdToMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "Player1MessageId",
                table: "Matches",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "Player2MessageId",
                table: "Matches",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Player1MessageId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Player2MessageId",
                table: "Matches");
        }
    }
}
