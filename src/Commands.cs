using Discord;
using Discord.WebSocket;
using Bot.Items;
using Bot.Utils;

namespace Bot.Commands
{
    public class Commands 
    {
        public static async Task SearchCommand(SocketSlashCommand command, IEnumerable<Item> itemsData)
        {
            string input = (string)command.Data.Options.First().Value;

            IEnumerable<Item> searchResult = Functions.SearchItem(itemsData, input);

            IEnumerable<string> requestResult = await Functions.RequestItem(searchResult);
            

            var embed = new EmbedBuilder
            {
                Title = "Busca",
                Color = Color.DarkBlue,
            };

            embed.AddField("Resultado", requestResult.Any() ? string.Join('\n', requestResult) : "Sem Resultados")
                .WithCurrentTimestamp();

            await command.RespondAsync(embed: embed.Build());
        }

        public static async Task TesteCommand(SocketSlashCommand command)
        {
            await command.RespondAsync("testado!");
        }

    } 
}