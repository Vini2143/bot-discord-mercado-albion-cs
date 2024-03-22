using DSharpPlus;
using DotNetEnv;
using Bot.Commands;
using Bot.Items;
using DSharpPlus.SlashCommands;

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

        await client.ConnectAsync();
        await Task.Delay(-1);
    }
}
