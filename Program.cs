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
        client.ButtonExecuted += ButtonHandler;

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

    private static async Task ButtonHandler(SocketMessageComponent component)
    {
        switch(component.Data.CustomId)
        {
            case "next-button":
                var embed = new EmbedBuilder
                {
                    Title = "Busca",
                    Color = Color.DarkBlue,
                };

                var buttons = new ComponentBuilder()
                    .WithButton("<<", "previous-button")
                    .WithButton(">>", "next-button");
                  
                
                await component.UpdateAsync(msg => {
                    msg.Content = "Proxima";
                    msg.Embed = embed.Build();
                    msg.Components = buttons.Build();
            });
            break;

            case "previous-button":
                embed = new EmbedBuilder
                {
                    Title = "Busca",
                    Color = Color.DarkBlue,
                };

                buttons = new ComponentBuilder()
                    .WithButton("<<", "previous-button")
                    .WithButton(">>", "next-button");
                  
                
                await component.UpdateAsync(msg => {
                    msg.Content = "Anterior";
                    msg.Embed = embed.Build();
                    msg.Components = buttons.Build();
            });
            break;
        }
    }
}