using Discord.WebSocket;
using Discord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent; // For thread-safe collections

public class GameLogic
{
    private readonly EloBotContext _context;
    private readonly DiscordSocketClient _client;
    private readonly RankSystem _rankSystem;
    private readonly ILogger<GameLogic> _logger;
    private ConcurrentQueue<(ulong userId, ulong channelId)> _queue = new();    // Thread-safe queue
    private const int K = 32; // Elo K-factor

    public GameLogic(EloBotContext context, DiscordSocketClient client, RankSystem rankSystem, ILogger<GameLogic> logger)
    {
        _context = context;
        _client = client;
        _rankSystem = rankSystem;
        _logger = logger;
    }

    public async Task<string> JoinQueue(ulong userId, ulong channelId)
    {
        await InitializePlayer(userId);

        if (_queue.Any(q => q.userId == userId))
            return "You are already in the queue.";

        _queue.Enqueue((userId, channelId));

        if (_queue.Count >= 2)
        {
            if (_queue.TryDequeue(out var player1) && _queue.TryDequeue(out var player2))
            {
                await CreateMatch(player1, player2);
                return "Match found! Starting game...";
            }
        }

        return "You have joined the queue. Waiting for opponent...";
    }

    public string LeaveQueue(ulong userId)
    {
        var updatedQueue = _queue.Where(q => q.userId != userId).ToList();
        _queue = new ConcurrentQueue<(ulong userId, ulong channelId)>(updatedQueue);
        return updatedQueue.Any(q => q.userId == userId) ? "You have left the queue." : "You were not in the queue.";
    }


    private async Task CreateMatch((ulong id, ulong channelId) player1, (ulong id, ulong channelId) player2)
    {
        var match = new Match
        {
            FirstPlayer = await _context.Players.FindAsync(player1.id),
            SecondPlayer = await _context.Players.FindAsync(player2.id),
            MatchDate = DateTime.UtcNow,
            FirstMessageChannelId = player1.channelId,
            SecondMessageChannelId = player2.channelId,
        };

        _context.Matches.Add(match);
        await _context.SaveChangesAsync();

        await NotifyPlayers(match);
    }


    private async Task NotifyPlayers(Match match)
    {
        var player1 = _client.GetUser(match.FirstPlayer.DiscordId);
        var player2 = _client.GetUser(match.SecondPlayer.DiscordId);

        if (player1 == null || player2 == null)
        {
            _logger.LogWarning("One or both players could not be found for match notification.");
            return;
        }

        var embed = new EmbedBuilder()
            .WithTitle("Match Found!")
            .WithDescription($"{player1.Mention} vs {player2.Mention}")
            .WithColor(Color.Green)
            .Build();

        var component = new ComponentBuilder()
            .WithButton("I Won", $"win:{match.MatchId}", ButtonStyle.Success)
            .WithButton("I Lost", $"lose:{match.MatchId}", ButtonStyle.Danger)
            .Build();

        var player1Channel = _client.GetChannel(match.FirstMessageChannelId) as IMessageChannel;
        var player2Channel = _client.GetChannel(match.SecondMessageChannelId) as IMessageChannel;

        var message1 = await player1Channel.SendMessageAsync(embed: embed, components: component);
        match.FirstMessageId = message1.Id;

        if (player1Channel.Id != player2Channel.Id)
        {
            var message2 = await player2Channel.SendMessageAsync(embed: embed, components: component);
            match.SecondMessageId = message2.Id;
        }
        else
        {
            match.SecondMessageId = message1.Id;
        }

        await _context.SaveChangesAsync();
    }



    public async Task HandleMatchConfirmation(SocketMessageComponent component)
    {
        var parts = component.Data.CustomId.Split(':');
        var action = parts[0];
        var matchId = int.Parse(parts[1]);

        var match = await _context.Matches.FindAsync(matchId);
        if (match == null)
        {
            await component.RespondAsync("This match doesn't exist.", ephemeral: true);
            return;
        }

        var userId = component.User.Id;
        if (userId != match.FirstPlayer.DiscordId && userId != match.SecondPlayer.DiscordId)
        {
            await component.RespondAsync("You are not a participant in this match.", ephemeral: true);
            return;
        }

        bool player1Confirming = userId == match.FirstPlayer.DiscordId;
        bool won = action == "win";

        if (player1Confirming)
        {
            match.FirstPlayerConfirmedWin = won;
        }
        else
        {
            match.SecondPlayerConfirmedWin = won;
        }

        // Check if both players have confirmed the same winner
        if (match.FirstPlayerConfirmedWin.HasValue && match.SecondPlayerConfirmedWin.HasValue)
        {
            if (match.FirstPlayerConfirmedWin != match.SecondPlayerConfirmedWin)
            {
                var winnerId = match.FirstPlayerConfirmedWin.Value ? match.FirstPlayer.DiscordId : match.SecondPlayer.DiscordId;
                var result = await ConfirmMatchOutcome(matchId, winnerId);
                //var result = "Test";

                var firstChannel = await _client.GetChannelAsync(match.FirstMessageChannelId) as IMessageChannel;
                var firstMessage = await firstChannel.GetMessageAsync(match.FirstMessageId) as IUserMessage;

                var secondChannel = await _client.GetChannelAsync(match.SecondMessageChannelId) as IMessageChannel;
                var secondMessage = await secondChannel.GetMessageAsync(match.SecondMessageId) as IUserMessage;

                // Modify the message
                await firstMessage.ModifyAsync(x =>
                {
                    x.Embed = new EmbedBuilder()
                        .WithTitle("Match Confirmed")
                        .WithDescription(result)
                        .WithColor(Color.Blue)
                        .Build();
                    x.Components = new ComponentBuilder().Build();
                });


                if (match.FirstMessageId != match.SecondMessageId)
                {
                    await secondMessage.ModifyAsync(x =>
                    {
                        x.Embed = new EmbedBuilder()
                            .WithTitle("Match Confirmed")
                            .WithDescription(result)
                            .WithColor(Color.Blue)
                            .Build();
                        x.Components = new ComponentBuilder().Build();
                    });
                }

            }
            else
            {
                // Reset confirmation if players disagree
                match.FirstPlayerConfirmedWin = null;
                match.SecondPlayerConfirmedWin = null;
                match.PendingWinnerId = null;

                await component.RespondAsync("Players disagree on the outcome. Please try again.", ephemeral: true);
            }
        }
        else
        {
            // Store the pending winner ID if this is the first confirmation
            if (match.PendingWinnerId == null)
            {
                match.PendingWinnerId = won ? userId : (userId == match.FirstPlayer.DiscordId ? match.SecondPlayer.DiscordId : match.FirstPlayer.DiscordId);
            }

            await _context.SaveChangesAsync();

            await component.RespondAsync("Your result has been recorded. Waiting for your opponent to confirm.", ephemeral: true);
        }

        await _context.SaveChangesAsync();
    }


    public async Task<string> ConfirmMatchOutcome(int matchId, ulong winnerId)
    {
        var match = await _context.Matches.FindAsync(matchId);
        if (match == null)
            return "Match not found.";

        if (match.WinnerId.HasValue)
            return "Match outcome already confirmed.";

        if (winnerId != match.FirstPlayer.DiscordId && winnerId != match.SecondPlayer.DiscordId)
            return "Invalid winner ID.";

        match.WinnerId = winnerId;

        var player1 = await _context.Players.FindAsync(match.FirstPlayer.DiscordId);
        var player2 = await _context.Players.FindAsync(match.SecondPlayer.DiscordId);

        if (player1 == null || player2 == null)
        {
            return "One or both players not found.";
        }

        var (player1EloChange, player2EloChange) = CalculateEloChange(player1.Elo, player2.Elo, winnerId == player1.DiscordId);

        player1.Elo += player1EloChange;
        player2.Elo += player2EloChange;

        match.EloChange = Math.Abs(player1EloChange); // Store absolute Elo change

        await _context.SaveChangesAsync();

        await UpdatePlayerRanks(player1);
        await UpdatePlayerRanks(player2);

        return $"Match outcome confirmed. {player1.Username}: {player1EloChange}, {player2.Username}: {player2EloChange}";
    }

    private (int, int) CalculateEloChange(int elo1, int elo2, bool player1Won)
    {
        double expectedScore1 = 1 / (1 + Math.Pow(10, (elo2 - elo1) / 400.0));
        double actualScore1 = player1Won ? 1 : 0;

        int change1 = (int)Math.Round(K * (actualScore1 - expectedScore1));
        return (change1, -change1);
    }

    public async Task<string> GetPlayerElo(ulong userId)
    {
        await InitializePlayer(userId);
        var player = await _context.Players.FindAsync(userId);
        if (player == null)
            return "Player not found.";

        return $"Your current Elo is: {player.Elo}";
    }

    public async Task<string> GetLeaderboard(int top = 10)
    {
        var topPlayers = await _context.Players
            .OrderByDescending(p => p.Elo)
            .Take(top)
            .ToListAsync();

        return string.Join("\n", topPlayers.Select((p, i) => $"{i + 1}. {p.Username}: {p.Elo}"));
    }

    public async Task<string> Forfeit(ulong userId)
    {
        var match = await _context.Matches
            .FirstOrDefaultAsync(m => (m.FirstPlayer.DiscordId == userId || m.SecondPlayer.DiscordId == userId) && !m.WinnerId.HasValue);

        if (match == null)
            return "You are not in an active match.";

        var winnerId = match.FirstPlayer.DiscordId == userId ? match.SecondPlayer.DiscordId : match.FirstPlayer.DiscordId;
        return await ConfirmMatchOutcome(match.MatchId, winnerId);
    }

    private async Task UpdatePlayerRanks(Player player)
    {
        try
        {
            var newRank = _rankSystem.GetRankForElo(player.Elo);
            if (newRank != player.Rank)
            {
                player.Rank = newRank;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Player {player.Username} rank updated to {newRank}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating rank for player {player.Username}");
        }
    }

    public async Task<string> SetPlayerRank(ulong userId, string username, string rank)
    {
        try
        {
            var player = await _context.Players
                .FirstOrDefaultAsync(p => p.DiscordId == userId);

            if (player == null)
            {
                player = new Player
                {
                    DiscordId = userId,
                    Username = username,
                    Elo = 1000
                };

                _context.Players.Add(player);
            }

            player.Rank = rank;
            await _context.SaveChangesAsync();

            return $"Successfully set {username}'s rank to {rank}.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error setting rank for user {userId}");
            return $"An error occurred while setting rank: {ex.Message}";
        }
    }

    public async Task<string> SetPlayerElo(ulong userId, string username, int elo)
    {
        try
        {
            var player = await _context.Players
                .FirstOrDefaultAsync(p => p.DiscordId == userId);

            if (player == null)
            {
                player = new Player
                {
                    DiscordId = userId,
                    Username = username,
                    Rank = "Unranked"
                };

                _context.Players.Add(player);
            }

            player.Elo = elo;
            var affectedRows = await _context.SaveChangesAsync();

            return $"Successfully set {username}'s Elo to {elo}. Database update affected {affectedRows} rows.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error setting Elo for user {userId}");
            return $"An error occurred while setting Elo: {ex.Message}";
        }
    }

    public async Task<string> ForceForfeit(ulong userId)
    {
        try
        {
            var match = await _context.Matches
                .FirstOrDefaultAsync(m => (m.FirstPlayer.DiscordId == userId || m.SecondPlayer.DiscordId == userId) && !m.WinnerId.HasValue);

            if (match == null)
            {
                Player player = await _context.Players.FindAsync(userId);
                return $"{player.Username} is not in an active match.";
            }

            var winnerId = match.FirstPlayer.DiscordId == userId ? match.SecondPlayer.DiscordId : match.FirstPlayer.DiscordId;
            var result = await ConfirmMatchOutcome(match.MatchId, winnerId);

            return $"Force forfeited match. {result}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error forcing forfeit for user {userId}");
            return $"An error occurred while forcing forfeit: {ex.Message}";
        }
    }

    private async Task InitializePlayer(ulong userId)
    {
        var player = await _context.Players.FindAsync(userId);
        if (player == null)
        {
            var discordUser = await _client.GetUserAsync(userId);
            if (discordUser == null)
            {
                _logger.LogWarning($"User {userId} not found on Discord.");
                return;
            }

            player = new Player
            {
                DiscordId = userId,
                Elo = 1000, // Default Elo rating
                Username = discordUser.Username,
                Rank = _rankSystem.GetRankForElo(1000) // Initialize rank based on default Elo
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"New player {userId} initialized with default Elo rating and rank.");
        }
    }
}
