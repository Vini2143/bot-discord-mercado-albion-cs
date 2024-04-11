using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Bot.Utils;
using Newtonsoft.Json;

namespace Bot.Commands
{   
    
    public sealed class Commands : ApplicationCommandModule
    {   
        [SlashCommand("search", "Busca um item pelo nome")]
        public async Task SearchCommand(InteractionContext ctx, [Option("busca", "Nome do item")] string input)
        {   
            Functions.SearchItem(input, out var inputItems, out var inputQualities);
            var requestResult = await Functions.RequestItem(inputItems, inputQualities, true);

            if (requestResult.Count == 0) 
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Sem resultados!"));
                return;
            }

            var interactivity = ctx.Client.GetInteractivity();

            List<Page> pages = [];

            try {
                foreach (var item in requestResult)
                {
                    var page = new Page("", new DiscordEmbedBuilder() 
                        .WithTitle(Functions.GetItem(item.Key).Name)
                        .AddField("Preço", string.Join('\n', item.Value[0]), true)
                        .AddField("Qualidade", string.Join('\n', item.Value[1]), true)
                        .AddField("Localização", string.Join('\n', item.Value[2]), true));

                    pages.Add(page);
                }

                await interactivity.SendPaginatedResponseAsync(ctx.Interaction, false, ctx.Member, pages);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro: " + ex.Message);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Ocorreu um erro!"));
            }
        }

        [SlashCommand("profitas", "Calcula o lucro")]
        public async Task ProfitasCommand(InteractionContext ctx, [Option("item", "Nome do item")] string input)
        {
            Functions.SearchItem(input, out var inputItems, out var inputQualities);
            Functions.SearchItemRecipe(inputItems,out var recipe, out var recipeItems);
            
            var requestResult = await Functions.RequestItem(recipeItems, []);

            if (requestResult.Count == 0) 
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Sem resultados!"));
                return;
            }

            var interactivity = ctx.Client.GetInteractivity();

            List<Page> pages = [];

            try {
                for(int index = 0; index <= 5; index++)
                {
                    var item = recipe.First();
                    var costs = recipe.Skip(1).Select(item => $"**{item.Value}x** {Functions.GetItem(item.Key).Name}(**{requestResult[item.Key][0][index]}**)");

                    var page = new Page("", new DiscordEmbedBuilder() 
                        .WithTitle($"Profit em {requestResult[item.Key][2][index]}")
                        .AddField("Produto", $"**{item.Value}x** {Functions.GetItem(item.Key).Name}(**{requestResult[item.Key][0][index]}**)")
                        .AddField("Custos", $"\n{string.Join('\n', costs)}"));
                    
                    pages.Add(page);
                }

                await interactivity.SendPaginatedResponseAsync(ctx.Interaction, false, ctx.Member, pages);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro: " + ex.ToString());
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