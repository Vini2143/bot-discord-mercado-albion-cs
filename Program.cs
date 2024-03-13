using Discord;
using Discord.WebSocket;
using DotNetEnv;
using Bot.Commands;
using Bot.CommandsData;
using Bot.Items;

public class Program
{
    private static DiscordSocketConfig config = new()
    {
        UseInteractionSnowflakeDate = false
    };
    private static DiscordSocketClient client = new(config);
    
    public static async Task Main()
    {
        Env.Load();
        var token = Environment.GetEnvironmentVariable("TOKEN");

        client.Log += ClientLog;
        client.Ready += ClientReady;
        client.SlashCommandExecuted += SlashCommandHandler;

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        await Task.Delay(-1);
    }

    private static Task ClientLog(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private static async Task ClientReady()
    {   
        await CommandsData.Load(client);
    }

    private static async Task SlashCommandHandler(SocketSlashCommand command)
    {   
        switch(command.Data.Name)
        {
            case "search":
                await Commands.SearchCommand(command, ItemsData.Instance.Data);
                break;
            
            case "teste":
                await Commands.TesteCommand(command);
                break;
        }
    }
}