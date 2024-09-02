using Discord;
using Discord.Interactions;
using System;
using System.Threading.Tasks;

public class AdminCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly GameLogic _gameLogic;

    public AdminCommands(GameLogic gameLogic)
    {
        _gameLogic = gameLogic;
    }

    [SlashCommand("setrank", "Set a player's rank")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task SetRank(IUser user, string rank)
    {
        if (user == null)
        {
            await RespondAsync("User cannot be null.");
            return;
        }

        var result = await _gameLogic.SetPlayerRank(user.Id, user.Username, rank);
        await RespondAsync(result);
    }

    [SlashCommand("setelo", "Set a player's Elo")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task SetElo(IUser user, int elo)
    {
        if (user == null)
        {
            await RespondAsync("User cannot be null.");
            return;
        }

        var result = await _gameLogic.SetPlayerElo(user.Id, user.Username, elo);
        await RespondAsync(result);
    }

    [SlashCommand("forceforfeit", "Force forfeit a match")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ForceForfeit(IUser user)
    {
        if (user == null)
        {
            await RespondAsync("User cannot be null.");
            return;
        }

        var result = await _gameLogic.ForceForfeit(user.Id);
        await RespondAsync(result);
    }
}
