using Discord;
using Discord.Net;
using Discord.WebSocket;
using DotNetEnv;
using Newtonsoft.Json;

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
        var globalCommand = new SlashCommandBuilder()
            .WithName("teste")
            .WithDescription("teste descrição");

        try 
        {
            await client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
        }
        catch(HttpException exception)
        {
            Console.WriteLine(exception);
        }
    }

    private static async Task SlashCommandHandler(SocketSlashCommand command)
    {   
        switch(command.Data.Name)
        {
            case "teste":
                await TesteCommand(command);
                break;
        }
    }

    private static async Task TesteCommand(SocketSlashCommand command)
    {
        await command.RespondAsync("testado!");
    }
}