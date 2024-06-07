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
        [SlashCommand("vendas", "Pesquisa os pedidos de venda")]
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
                        .WithThumbnail($"https://render.albiononline.com/v1/item/{Functions.GetItem(item.Key).Code}.png")
                        .WithTitle($"{Functions.GetItem(item.Key).Name} {Functions.GetItem(item.Key).Tier}.{Functions.GetItem(item.Key).Enchant}")
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

        [SlashCommand("compras", "Pesquisa os pedidos de compra")]
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
                        .WithThumbnail($"https://render.albiononline.com/v1/item/{item.Key}.png")
                        .WithTitle($"{Functions.GetItem(item.Key).Name} {Functions.GetItem(item.Key).Tier}.{Functions.GetItem(item.Key).Enchant}")
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

        [SlashCommand("profitas", "Busca o preço dos recursos usados na produção")]
        public async Task ProfitasCommand(InteractionContext ctx, [Option("item", "Nome do item")] string input)
        {
            try
            {
                Functions.SearchItem(input, out var inputItems, out var inputQualities);
                var product = inputItems.First();
                var resourceList = await Functions.RequestItemRecipe(product);

                IEnumerable<Item> recipeItems = resourceList.Select(item => Functions.GetItem(item.uniqueName)).Append(inputItems.First());

                
                var requestResult = await Functions.RequestItem(recipeItems, [1], ["Bridgewatch", "Caerleon", "Fort Sterling", "Lymhurst", "Martlock", "Thetford", "Brecilien"]);
                if (requestResult.Count == 0) 
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Sem resultados!"));
                    return;
                }

                var interactivity = ctx.Client.GetInteractivity();

                List<Page> pages = [];
                

                /* for(int index = 0; index <= 5; index++)
                {
                    var item = recipe.First();
                    var costs = recipe.Skip(1).Select(item => $"**{item.Value}x** {Functions.GetItem(item.Key).Name}(**{requestResult[item.Key][2][index]}**)");
                    var totalCost = recipe.Skip(1).Select(item => item.Value * Int32.Parse(requestResult[item.Key][2][index]));

                    var page = new Page("", new DiscordEmbedBuilder() 
                        .WithTitle($"Profit em {requestResult[item.Key][0][index]}")
                        .AddField("Produto", $"**{item.Value}x** {Functions.GetItem(item.Key).Name}(**{requestResult[item.Key][2][index]}**)")
                        .AddField("Custos", $"\n{string.Join('\n', costs)}\n**Custo Total:**{totalCost.Sum()}"));
                    
                    pages.Add(page);
                }*/
                string productCode = product.Enchant == "0" ? product.Code : $"{product.Code}@{product.Enchant}";
                var page1 = new Page("", new DiscordEmbedBuilder() 
                    .WithThumbnail($"https://render.albiononline.com/v1/item/{productCode}.png")
                    .WithTitle($"{product.Name} {product.Tier}.{product.Enchant}")
                    .AddField("Localização", string.Join('\n',  requestResult[productCode][0]), true)
                    .AddField("Pedido venda", string.Join('\n',  requestResult[productCode][2]), true)
                    .AddField("Pedido compra", string.Join('\n',  requestResult[productCode][3]), true));

                pages.Add(page1);

                requestResult.Remove(productCode);

                foreach (var item in requestResult)
                {
                    var page = new Page("", new DiscordEmbedBuilder() 
                        .WithThumbnail($"https://render.albiononline.com/v1/item/{item.Key}.png")
                        .WithTitle($"x {Functions.GetItem(item.Key).Name} {Functions.GetItem(item.Key).Tier}.{Functions.GetItem(item.Key).Enchant}")
                        .AddField("Localização", string.Join('\n', item.Value[0]), true)
                        .AddField("Pedido venda", string.Join('\n', item.Value[2]), true)
                        .AddField("Pedido compra", string.Join('\n', item.Value[3]), true));

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