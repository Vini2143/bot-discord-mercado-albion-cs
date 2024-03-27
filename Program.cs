using DSharpPlus;
using DotNetEnv;
using Bot.Commands;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity;

public class Program
{
    private static DiscordConfiguration config = new()
    {
        Token = Environment.GetEnvironmentVariable("TOKEN"),
        TokenType = TokenType.Bot
    };
    private static DiscordClient client = new(config);

    public static async Task Main()
    {
        Env.Load();

        var slash = client.UseSlashCommands();
        slash.RegisterCommands<Commands>();

        client.UseInteractivity(new InteractivityConfiguration() 
        { 
            PollBehaviour = PollBehaviour.KeepEmojis,
            Timeout = TimeSpan.FromSeconds(30)
        });
        

        await client.ConnectAsync();
        await Task.Delay(-1);
    }
}
