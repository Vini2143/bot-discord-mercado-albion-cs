using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Bot.Items;
using Bot.Utils;

namespace Bot.Commands
{
    public class Commands : ApplicationCommandModule
    {   
        [SlashCommand("search", "Busca um item pelo nome")]
        public async Task SearchCommand(InteractionContext ctx, [Option("busca", "Nome do item")] string input)
        {

            IEnumerable<Item> searchResult = Functions.SearchItem(ItemsData.Instance.Data, input);

            var requestResult = await Functions.RequestItem(searchResult);

            var msg = new DiscordInteractionResponseBuilder();

            var embed = new DiscordEmbedBuilder
            {
                Title = "Busca",
            };

            
            if (requestResult["name"].Count == 0) {
                embed.WithDescription("Sem Resultados!");

            } else {
                embed.AddField("Nome", string.Join('\n', requestResult["name"]), true);
                embed.AddField("Cidade", string.Join('\n', requestResult["city"]), true);
                embed.AddField("Preço - Tempo", string.Join('\n', requestResult["price-time"]), true);

            }

            msg.AddEmbed(embed);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, msg);

        }

        [SlashCommand("teste", "testa algo")]
        public async Task TesteCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Testado!"));
        }

    } 
}