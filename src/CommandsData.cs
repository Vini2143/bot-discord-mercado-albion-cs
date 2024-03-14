using Discord;
using Discord.Net;
using Discord.WebSocket;

namespace Bot.CommandsData 
{
    public class CommandsData {

        public static async Task Load(DiscordSocketClient client) {
            
            try 
            {
                await client.CreateGlobalApplicationCommandAsync(searchCommandData.Build());
                await client.CreateGlobalApplicationCommandAsync(testeCommandData.Build());
            }
            catch(HttpException exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static SlashCommandBuilder searchCommandData = new SlashCommandBuilder()
            .WithName("search")
            .WithDescription("Busca item pelo nome")
            .AddOption("busca", ApplicationCommandOptionType.String, "nome do item", isRequired: true);
            
        private static SlashCommandBuilder testeCommandData = new SlashCommandBuilder()
            .WithName("teste")
            .WithDescription("teste descrição");
    } 
}