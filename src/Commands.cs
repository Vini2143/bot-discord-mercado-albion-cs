using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Bot.Items;
using Bot.Utils;

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
            if (!requestResult.Any()) 
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Sem resultados!"));
                return;
            }

            var interactivity = ctx.Client.GetInteractivity();

            List<Page>pages = [];

            try {
                foreach (var item in requestResult)
                {
                    var page = new Page("", new DiscordEmbedBuilder() 
                        .WithTitle(item.Key)
                        .AddField("Preço", string.Join('\n', item.Value[0]), true)
                        .AddField("Qualidade", string.Join('\n', item.Value[1]), true)
                        .AddField("Localização-Tempo", string.Join('\n', item.Value[2]), true));

                    pages.Add(page);
                }

                await interactivity.SendPaginatedResponseAsync(ctx.Interaction , false ,ctx.Member, pages);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro: " + ex.Message);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Ocorreu um erro!"));
            }
        }

        [SlashCommand("teste", "testa algo")]
        public async Task TesteCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Testado!"));
        }

    } 
}