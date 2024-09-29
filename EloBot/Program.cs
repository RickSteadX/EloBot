using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.IO;
using Microsoft.Extensions.Logging;

class Program
{
    private DiscordSocketClient _client;
    private InteractionService _interactions;
    private IServiceProvider _services;
    private IConfiguration _configuration;

    public static Task Main(string[] args) => new Program().MainAsync();

    public async Task MainAsync()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        // Build configuration
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .Build();

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(_configuration) // Reads from appsettings.json / appsettings.Development.json
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day) // Configures file logging
            .WriteTo.Console()
            .Enrich.FromLogContext()
            .CreateLogger(); // Configures console logging

        // Set up dependency injection
        _services = ConfigureServices();

        // Set up the Discord client and interactions
        _client = _services.GetRequiredService<DiscordSocketClient>();
        _interactions = _services.GetRequiredService<InteractionService>();
        await _interactions.AddModulesAsync(assembly: typeof(UserCommands).Assembly, services: _services);


        // Hook up Serilog logging
        _client.Log += LogAsync;
        _interactions.Log += LogAsync;

        _client.Ready += ReadyAsync;
        _client.InteractionCreated += HandleInteraction;

        // Login and start the client
        await _client.LoginAsync(TokenType.Bot, _configuration["DiscordToken"]);
        await _client.StartAsync();

        await Task.Delay(-1);
    }

    private async Task ReadyAsync()
    {
        Log.Information($"Logged in as {_client.CurrentUser.Username}#{_client.CurrentUser.Discriminator}");
        ulong guildID = ulong.Parse(_configuration["ServerID"]);
        await _interactions.RegisterCommandsToGuildAsync(guildID);
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            if (interaction is SocketMessageComponent component)
            {
                var gameLogic = _services.GetRequiredService<GameLogic>();
                await gameLogic.HandleMatchConfirmation(component);
            }
            else
            {
                var context = new SocketInteractionContext(_client, interaction);
                await _interactions.ExecuteCommandAsync(context, _services);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while handling interaction");

            if (interaction.Type == InteractionType.ApplicationCommand)
            {
                await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }

    private Task LogAsync(LogMessage log)
    {
        Log.Information(log.ToString());
        return Task.CompletedTask;
    }

    private ServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton(_configuration)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<GameLogic>()
            .AddSingleton<RankSystem>()
            .AddDbContext<EloBotContext>(options =>
                options
                .UseSqlite(_configuration.GetConnectionString("DefaultConnection"))
                //.LogTo(Console.WriteLine, LogLevel.Information) // If you want to log every SQL query
                )
            .AddLogging(builder =>
            {
                builder.ClearProviders(); // Remove other logging providers
                builder.AddSerilog(); // Add Serilog
            })
            .BuildServiceProvider();
    }
}
