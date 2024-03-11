using Discord.WebSocket;
using Sprache;
using Bot.Items;

namespace Bot.Commands {
    public class Commands {

        public static async Task SlashCommandHandler(SocketSlashCommand command)
        {   
        switch(command.Data.Name)
            {
                case "search":
                    await SearchCommand(command);
                    break;
                
                case "teste2":
                    await Teste2Command(command);
                    break;
            }
        }

        private static async Task SearchCommand(SocketSlashCommand command)
        {   
            var itemsData = ItemsData.Instance.GetData();

            string search = (string)command.Data.Options.First().Value;

            var result = from itemData in itemsData
                        where itemData.name.Contains(search)
                        select itemData;

            foreach (Item item in result) {
                Console.WriteLine($"{item.name} {item.tier}");
            }

            await command.RespondAsync("testado 1!");
        }

        private static async Task Teste2Command(SocketSlashCommand command)
        {
            await command.RespondAsync("testado 2!");
        }
    } 
}