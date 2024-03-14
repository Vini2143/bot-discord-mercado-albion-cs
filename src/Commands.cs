using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Bot.Items;
using Bot.Utils;

namespace Bot.Commands
{
    public class Commands {
        public static async Task SearchCommand(SocketSlashCommand command, List<Item> itemsData)
        {
            string input = (string)command.Data.Options.First().Value;

            var tiers = Regex.Matches(input, @"\d.\d").Select(match => match.Value);

            input = Regex.Replace(input, @"\d.\d", "");
            input = input.Trim();

            IEnumerable<string> result = Functions.SearchItem(itemsData, input, tiers);

            string list = string.Join('\n', result);

            var embed = new EmbedBuilder()
                .WithTitle("Resultado")
                .WithDescription(list)
                .WithColor(Color.DarkBlue)
                .WithCurrentTimestamp();

            await command.RespondAsync(embed: embed.Build());
        }

        public static async Task TesteCommand(SocketSlashCommand command)
        {
            await command.RespondAsync("testado!");
        }
    } 
}