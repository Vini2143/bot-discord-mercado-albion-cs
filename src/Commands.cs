using Discord.WebSocket;
using Bot.Items;
using Discord;

namespace Bot.Commands {
    public class Commands {
        public static async Task SearchCommand(SocketSlashCommand command, List<Item> itemsData)
        {
            string search = (string)command.Data.Options.First().Value;

            var result = from itemData in itemsData
                        where itemData.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                        select $"{itemData.Name} {itemData.Tier}";
            
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