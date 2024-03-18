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

            var requestResult = await Functions.RequestItem(searchResult);
            

            var embed = new EmbedBuilder
            {
                Title = "Busca",
                Color = Color.DarkBlue,
            };
            
            if (!requestResult["name"].Any()) {
                await command.RespondAsync("Sem Resultados!");
                return;
            }
            
            embed.AddField("Nome", string.Join('\n', requestResult["name"]), true);
            embed.AddField("Cidade", string.Join('\n', requestResult["city"]), true);
            embed.AddField("Preço - Tempo", string.Join('\n', requestResult["price-time"]), true);

            await command.RespondAsync(embed: embed.Build());
        }

        public static async Task TesteCommand(SocketSlashCommand command)
        {
            await command.RespondAsync("testado!");
        }

    } 
}