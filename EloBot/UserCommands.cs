using Discord;
using Discord.Interactions;
using System;
using System.Threading.Tasks;

public class UserCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly GameLogic _gameLogic;
    private readonly RankSystem _rankSystem;

    public UserCommands(GameLogic gameLogic, RankSystem rankSystem)
    {
        _gameLogic = gameLogic;
        _rankSystem = rankSystem;
    }

    [SlashCommand("queue", "Join or leave the queue")]
    public async Task Queue([Choice("join", "join"), Choice("leave", "leave")] string action)
    {
        var userId = Context.User.Id;

        try
        {
            string response = action == "join"
                ? await _gameLogic.JoinQueue(userId, Context.Interaction.ChannelId.Value)
                : _gameLogic.LeaveQueue(userId);

            await RespondAsync(response);
        }
        catch (Exception ex)
        {
            await RespondAsync($"An error occurred while processing your request: {ex.Message}");
        }
    }

    [SlashCommand("elo", "Display your current Elo")]
    public async Task Elo()
    {
        var userId = Context.User.Id;

        try
        {
            var response = await _gameLogic.GetPlayerElo(userId);
            await RespondAsync(response);
        }
        catch (Exception ex)
        {
            await RespondAsync($"An error occurred while retrieving your Elo: {ex.Message}");
        }
    }

    [SlashCommand("leaderboard", "Show top players")]
    public async Task Leaderboard([MinValue(1)][MaxValue(100)] int top = 10)
    {
        try
        {
            var response = await _gameLogic.GetLeaderboard(top);
            await RespondAsync(response);
        }
        catch (Exception ex)
        {
            await RespondAsync($"An error occurred while fetching the leaderboard: {ex.Message}");
        }
    }

    [SlashCommand("ranks", "Display available ranks")]
    public async Task Ranks()
    {
        try
        {
            var response = _rankSystem.GetAvailableRanks();
            await RespondAsync(response);
        }
        catch (Exception ex)
        {
            await RespondAsync($"An error occurred while retrieving ranks: {ex.Message}");
        }
    }

    [SlashCommand("forfeit", "Forfeit your current match")]
    public async Task Forfeit()
    {
        var userId = Context.User.Id;

        try
        {
            var response = await _gameLogic.Forfeit(userId);
            await RespondAsync(response);
        }
        catch (Exception ex)
        {
            await RespondAsync($"An error occurred while processing the forfeit: {ex.Message}");
        }
    }
}
