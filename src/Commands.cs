using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Bot.Items;
using Bot.Utils;

namespace Bot.Commands
{
    public partial class Commands {
        public static async Task SearchCommand(SocketSlashCommand command, IEnumerable<Item> itemsData)
        {
            string input = (string)command.Data.Options.First().Value;

            var tiers = MyRegex().Matches(input).Select(match => match.Value);

            input = MyRegex().Replace(input, "");
            input = input.Trim();

            IEnumerable<Item> result = Functions.SearchItem(itemsData, input, tiers);

            string list = string.Join('\n', result);
            await Functions.RequestItem(result);

            var embed = new EmbedBuilder()
                .WithTitle("Resultado")
                .WithDescription("descrição")
                .WithColor(Color.DarkBlue)
                .WithCurrentTimestamp();

            await command.RespondAsync(embed: embed.Build());
        }

        public static async Task TesteCommand(SocketSlashCommand command)
        {
            await command.RespondAsync("testado!");
        }

        [GeneratedRegex(@"\d.\d")]
        private static partial Regex MyRegex();
    } 
}