using Discord;
using Discord.Net;
using Discord.WebSocket;

namespace Bot.CommandsData {
    public class CommandsData {

        public static async Task Load(DiscordSocketClient client) {
            
            try 
            {
                await client.CreateGlobalApplicationCommandAsync(TesteCommandData.Build());
                await client.CreateGlobalApplicationCommandAsync(Teste2CommandData.Build());
            }
            catch(HttpException exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static SlashCommandBuilder TesteCommandData = new SlashCommandBuilder()
            .WithName("teste1")
            .WithDescription("teste descrição 1");
        
        private static SlashCommandBuilder Teste2CommandData = new SlashCommandBuilder()
            .WithName("teste2")
            .WithDescription("teste descrição 2");
    } 
}