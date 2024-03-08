
using Discord.WebSocket;

namespace Bot.Commands {
    public class Commands {

        public static async Task SlashCommandHandler(SocketSlashCommand command)
        {   
        switch(command.Data.Name)
            {
                case "teste1":
                    await Teste1Command(command);
                    break;
                
                case "teste2":
                    await Teste2Command(command);
                    break;
            }
        }

        private static async Task Teste1Command(SocketSlashCommand command)
        {
            await command.RespondAsync("testado 1!");
        }

        private static async Task Teste2Command(SocketSlashCommand command)
        {
            await command.RespondAsync("testado 2!");
        }
    } 
}