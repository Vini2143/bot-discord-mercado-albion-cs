using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Sprache;

namespace Bot.Utils
{   
    public sealed class MarketData 
    {
        [JsonProperty("item_id")]
        public required string ItemId { get; set; }
        [JsonProperty("city")]
        public required string City { get; set; }
        [JsonProperty("quality")]
        public int Quality { get; set; }
        [JsonProperty("sell_price_min")]
        public int SellPriceMin { get; set; }
        [JsonProperty("sell_price_min_date")]
        public DateTime SellPriceMinDate { get; set; }
        [JsonProperty("sell_price_max")]
        public int SellPriceMax { get; set; }
        [JsonProperty("sell_price_max_date")]
        public DateTime SellPriceMaxDate { get; set; }
        [JsonProperty("buy_price_min")]
        public int BuyPriceMin { get; set; }
        [JsonProperty("buy_price_min_date")]
        public DateTime BuyPriceMinDate { get; set; }
        [JsonProperty("buy_price_max")]
        public int BuyPriceMax { get; set; }
        [JsonProperty("buy_price_max_date")]
        public DateTime BuyPriceMaxDate { get; set; }
    }
    public sealed class Item
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
        public required string Tier { get; set; }
    }

    public sealed partial class Functions
    {   
        private static readonly Dictionary<string, Item> itemsData = JsonConvert.DeserializeObject<Dictionary<string, Item>>(File.ReadAllText("./src/data/ItemsData.json"))!;
        private static readonly Dictionary<string, Dictionary<string, int>> recipesData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(File.ReadAllText("./src/data/RecipesData.json"))!;
        private static readonly Dictionary<int, string> qualitiesName = new()
        {
            {1, "Normal"},
            {2, "Bom"},
            {3, "Excepcional"},
            {4, "Excelente"},
            {5, "Obra-Prima"}
        };
        private static readonly Dictionary<string, int> qualitiesCode = new()
        {
            {"normal", 1},
            {"bom", 2},
            {"excepcional", 3},
            {"excelente", 4},
            {"obra-prima", 5},
            {"obra prima", 5},
            {"exp", 3},
            {"exc", 4},
            {"excep", 3},
            {"excel", 4},
        };
        private static readonly Dictionary<string, string> cities = new()
        {
            {"Bridgewatch", "Bridgewatch"},
            {"Caerleon", "Caerleon"},
            {"Fort Sterling", "Fort Sterling"},
            {"Lymhurst", "Lymhurst"},
            {"Martlock", "Martlock"},
            {"Thetford", "Thetford"},
            {"Black Market", "Black Market"},
            {"5003", "Brecilien"},
        };
            
        public static Item GetItem(string code)
        {
            return itemsData[code];
        }
        public static void SearchItem(string input, out IEnumerable<Item> result,  out IEnumerable<int> inputQualities)
        {
            var inputTiers = TierRegex().Matches(input).Select(match => {
                string tier = match.Groups["tier"].Value;
                string enchant = string.IsNullOrEmpty(match.Groups["enchant"].Value)? ".0":  match.Groups["enchant"].Value;
                
                return $"{tier}{enchant}";
            });
            inputQualities = QualityRegex().Matches(input).Select(match => qualitiesCode[match.Value.ToLower()]);
            
            input = TierRegex().Replace(input, "");
            input = QualityRegex().Replace(input, "");
            input = PrepositionRegex().Replace(input, "");
            input = CharRegex().Replace(input, "");

            Regex wordsRegex = new($@"(?=.*{string.Join(")(?=.*", input.Split(" ", StringSplitOptions.RemoveEmptyEntries))})", RegexOptions.IgnoreCase);

            if (inputTiers.Any())
            {   
                result = itemsData
                    .Where(item => wordsRegex.IsMatch(item.Value.Name) && inputTiers.Contains(item.Value.Tier))
                    .Select(item => item.Value);
                return;
            }
        
            result = result = itemsData
                    .Where(item => wordsRegex.IsMatch(item.Value.Name))
                    .Select(item => item.Value);
            return;
        }

        public static void SearchItemRecipe(IEnumerable<Item> items, out Dictionary<string, int> recipe, out IEnumerable<Item> recipeItems)
        {   
            recipe = items.Where(item => recipesData.ContainsKey(item.Code)).Select(item => recipesData[item.Code]).First();
            recipeItems = recipe.Keys.Select(itemCode => itemsData[itemCode]);
        }

        public static async Task<Dictionary<string, List<List<string>>>> RequestItem(IEnumerable<Item> items, IEnumerable<int> inputQualities, string? filterOfSearch = null)
        {            
            string apiUrl = $"https://west.albion-online-data.com/api/v2/stats/prices/{string.Join(",", items.Select(item => item.Code))}";

            if(inputQualities.Any())
            {
                apiUrl += $"?qualities={string.Join(',', inputQualities)}";
            }

            HttpClient client = new();

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();
            
                string responseBody = await response.Content.ReadAsStringAsync();

                IEnumerable<MarketData> list = JsonConvert.DeserializeObject<IEnumerable<MarketData>>(responseBody)!;

                DateTime now = DateTime.UtcNow;

                IEnumerable<MarketData> result = filterOfSearch switch
                {
                    "sell" => from item in list
                        where cities.ContainsKey(item.City) && item.SellPriceMin != 0
                        orderby item.SellPriceMin ascending
                        select item,
                    
                    "buy" => from item in list
                        where cities.ContainsKey(item.City) && item.BuyPriceMin != 0
                        orderby item.BuyPriceMax descending
                        select item,
                    
                    _ => from item in list
                        where cities.ContainsKey(item.City)
                        orderby item.SellPriceMin ascending
                        select item,
                };

                Dictionary<string, List<List<string>>> data = [];

                foreach (var item in result) {
                    if(!data.TryGetValue(item.ItemId, out List<List<string>>? value))
                    {
                        value = ([[], [], [], [], [], []]);
                        data[item.ItemId] = value;
                    }
                    
                    value[0].Add(cities[item.City]);
                    value[1].Add($"{qualitiesName[item.Quality]}");

                    value[2].Add(item.SellPriceMin.ToString());
                    value[3].Add(item.BuyPriceMax.ToString());
                    
                    value[4].Add($"há {(now - item.SellPriceMinDate).Hours:D2}:{(now - item.SellPriceMinDate).Minutes:D2} atrás");
                    value[5].Add($"há {(now - item.BuyPriceMaxDate).Hours:D2}:{(now - item.BuyPriceMaxDate).Minutes:D2} atrás");
                }

                return data;
        
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro: " + ex.ToString());
                return [];
            }
        }
        
        [GeneratedRegex(@"[^\p{L}\s-]")]
        private static partial Regex CharRegex();
        [GeneratedRegex(@"(^|\s)(de|do|da)(\s|$)", RegexOptions.IgnoreCase)]
        private static partial Regex PrepositionRegex();
        [GeneratedRegex(@"t?(?<tier>[1-8])(?<enchant>.[0-4])?", RegexOptions.IgnoreCase)]
        private static partial Regex TierRegex();
        [GeneratedRegex(@"(Normal|Bom|Excepcional|Excelente|Obra-Prima|Obra Prima|Excep|Excel|Exp|Exc)", RegexOptions.IgnoreCase)]
        private static partial Regex QualityRegex();

    }
}