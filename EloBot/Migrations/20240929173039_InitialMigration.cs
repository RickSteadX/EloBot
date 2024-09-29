using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EloBot.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    DiscordId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Elo = table.Column<int>(type: "INTEGER", nullable: false),
                    Rank = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.DiscordId);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    MatchId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstPlayerDiscordId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    SecondPlayerDiscordId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MatchDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WinnerId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    EloChange = table.Column<int>(type: "INTEGER", nullable: true),
                    FirstPlayerConfirmedWin = table.Column<bool>(type: "INTEGER", nullable: true),
                    SecondPlayerConfirmedWin = table.Column<bool>(type: "INTEGER", nullable: true),
                    PendingWinnerId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    FirstMessageId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    FirstMessageChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    SecondMessageId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    SecondMessageChannelId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.MatchId);
                    table.ForeignKey(
                        name: "FK_Matches_Players_FirstPlayerDiscordId",
                        column: x => x.FirstPlayerDiscordId,
                        principalTable: "Players",
                        principalColumn: "DiscordId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matches_Players_SecondPlayerDiscordId",
                        column: x => x.SecondPlayerDiscordId,
                        principalTable: "Players",
                        principalColumn: "DiscordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_FirstPlayerDiscordId",
                table: "Matches",
                column: "FirstPlayerDiscordId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_SecondPlayerDiscordId",
                table: "Matches",
                column: "SecondPlayerDiscordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
