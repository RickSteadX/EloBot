using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

public class EloBotContext : DbContext
{
    public DbSet<Player> Players { get; set; }
    public DbSet<Match> Matches { get; set; }

    public EloBotContext(DbContextOptions<EloBotContext> options)
        : base(options)
    {
    }
}

public class Player
{
    [Key]
    public ulong DiscordId { get; set; }
    public string Username { get; set; }
    public int Elo { get; set; }
    public string Rank { get; set; }
}

public class Match
{
    [Key]
    public int MatchId { get; set; }
    public Player FirstPlayer { get; set; }
    public Player SecondPlayer { get; set; }
    //public ulong FirstPlayerId { get; set; }
    //public ulong SecondPlayerId { get; set; }
    public DateTime MatchDate { get; set; }
    public ulong? WinnerId { get; set; }
    public int? EloChange { get; set; }
    public bool? FirstPlayerConfirmedWin { get; set; }
    public bool? SecondPlayerConfirmedWin { get; set; }
    public ulong? PendingWinnerId { get; set; }
    public ulong FirstMessageId { get; set; }
    public ulong FirstMessageChannelId { get; set; }
    public ulong SecondMessageId { get; set; }
    public ulong SecondMessageChannelId { get; set; }
}

