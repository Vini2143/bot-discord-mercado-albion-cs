using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Bot.Utils;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json;

namespace Bot.Commands
{   
    
    public sealed class Commands : ApplicationCommandModule
    {   
        [SlashCommand("compra", "Pesquisa os pedidos de venda")]
        public async Task SearchSellCommand(InteractionContext ctx, [Option("busca", "Nome do item")] string input)
        {   
            try
            {   
                Functions.SearchItem(input, out var inputItems, out var inputQualities);
                if (!inputItems.Any())
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Item não encontrado!"));
                    return;
                }

                var requestResult = await Functions.RequestItem(inputItems, inputQualities, [], "sell");
                if (requestResult.Count == 0)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Sem resultados!"));
                    return;
                }

                var interactivity = ctx.Client.GetInteractivity();
                List<Page> pages = [];

                foreach (var item in requestResult)
                {
                    var page = new Page("", new DiscordEmbedBuilder() 
                        .WithTitle($"{Functions.GetItem(item.Key).Name} {Functions.GetItem(item.Key).Tier}")
                        .AddField("Preço", string.Join('\n', item.Value[2]), true)
                        .AddField("Qualidade", string.Join('\n', item.Value[1]), true)
                        .AddField("Localização", string.Join('\n', item.Value[0]), true));

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

        [SlashCommand("venda", "Pesquisa os pedidos de compra")]
        public async Task SearchBuyCommand(InteractionContext ctx, [Option("busca", "Nome do item")] string input)
        {   
            try
            {
                Functions.SearchItem(input, out var inputItems, out var inputQualities);
                if (!inputItems.Any())
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Item não encontrado!"));
                    return;
                }

                var requestResult = await Functions.RequestItem(inputItems, inputQualities, [], "buy");
                if (requestResult.Count == 0) 
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Sem resultados!"));
                    return;
                }

                var interactivity = ctx.Client.GetInteractivity();
                List<Page> pages = [];

                foreach (var item in requestResult)
                {
                    var page = new Page("", new DiscordEmbedBuilder() 
                        .WithTitle($"{Functions.GetItem(item.Key).Name} {Functions.GetItem(item.Key).Tier}")
                        .AddField("Preço", string.Join('\n', item.Value[3]), true)
                        .AddField("Qualidade", string.Join('\n', item.Value[1]), true)
                        .AddField("Localização", string.Join('\n', item.Value[0]), true));

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
            try
            {
                Functions.SearchItem(input, out var inputItems, out var inputQualities);
                Functions.SearchItemRecipe(inputItems,out var recipe, out var recipeItems);
                
                var requestResult = await Functions.RequestItem(recipeItems, [], ["Bridgewatch", "Caerleon", "Fort Sterling", "Lymhurst", "Martlock", "Thetford"]);

                if (requestResult.Count == 0) 
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Sem resultados!"));
                    return;
                }

                var interactivity = ctx.Client.GetInteractivity();

                List<Page> pages = [];

                Console.WriteLine(JsonConvert.SerializeObject(requestResult, Formatting.Indented));

                for(int index = 0; index <= 5; index++)
                {
                    var item = recipe.First();
                    var costs = recipe.Skip(1).Select(item => $"**{item.Value}x** {Functions.GetItem(item.Key).Name}(**{requestResult[item.Key][2][index]}**)");

                    var page = new Page("", new DiscordEmbedBuilder() 
                        .WithTitle($"Profit em {requestResult[item.Key][0][index]}")
                        .AddField("Produto", $"**{item.Value}x** {Functions.GetItem(item.Key).Name}(**{requestResult[item.Key][2][index]}**)")
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