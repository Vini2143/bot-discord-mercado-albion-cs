using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity;
using Bot.Items;
using Bot.Utils;
using DSharpPlus.Interactivity.Extensions;

namespace Bot.Commands
{   
    
    public class Commands : ApplicationCommandModule
    {   
        private readonly IEnumerable<Item> data = ItemsData.Instance.Data;
        
        [SlashCommand("search", "Busca um item pelo nome")]
        public async Task SearchCommand(InteractionContext ctx, [Option("busca", "Nome do item")] string input)
        {

            IEnumerable<Item> searchResult = Functions.SearchItem(data, input);

            var requestResult = await Functions.RequestItem(searchResult);

            var interactivity = ctx.Client.GetInteractivity();
            
            if (requestResult["name"].Count == 0) 
            {
                await ctx.DeferAsync();
                return;
            }

            List<Page>pages = [];

            for(int pageNum = 0; pageNum * 10 <= requestResult["name"].Count; pageNum++) {
                var page = new Page("", new DiscordEmbedBuilder()
                    .AddField("Nome", string.Join('\n', requestResult["name"].Skip(pageNum * 10).Take(10)), true)
                    .AddField("Cidade", string.Join('\n', requestResult["city"].Skip(pageNum * 10).Take(10)), true)
                    .AddField("Preço - Tempo", string.Join('\n', requestResult["price-time"].Skip(pageNum * 10).Take(10)), true));

                pages.Add(page);
            }
           
            await interactivity.SendPaginatedResponseAsync(ctx.Interaction , false ,ctx.Member, pages);

        }

        [SlashCommand("teste", "testa algo")]
        public async Task TesteCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Testado!"));
        }

    } 
}