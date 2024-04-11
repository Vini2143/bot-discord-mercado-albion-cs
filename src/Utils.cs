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
            {3, "Exepcional"},
            {4, "Excelente"},
            {5, "Obra-Prima"}
        };
        private static readonly Dictionary<string, int> qualitiesCode = qualitiesName.ToDictionary(x => x.Value.ToLower(), x => x.Key);
        private static readonly IEnumerable<string> cities = ["Bridgewatch", "Caerleon", "Fort Sterling", "Lymhurst", "Martlock", "Thetford"];
        
        public static Item GetItem(string code)
        {
            return itemsData[code];
        }
        public static void SearchItem(string input, out IEnumerable<Item> result,  out IEnumerable<string> inputQualities)
        {
            
            var inputTiers = TierRegex().Matches(input).Select(match => match.Value);
            inputQualities = QualityRegex().Matches(input).Select(match => match.Value.ToLower());

            input = QualityRegex().Replace(input, "");
            input = CharRegex().Replace(input, "");
            input = input.Trim();

            if (inputTiers.Any())
            {   
                result = from item in itemsData
                    where item.Value.Name.Contains(input, StringComparison.OrdinalIgnoreCase) && inputTiers.Contains(item.Value.Tier)
                    select item.Value;
                return;
            }
        
            result = from item in itemsData
                where item.Value.Name.Contains(input, StringComparison.OrdinalIgnoreCase) 
                select item.Value;
            return;
        }

        public static void SearchItemRecipe(IEnumerable<Item> items, out Dictionary<string, int> recipe, out IEnumerable<Item> recipeItems)
        {   
            recipe = items.Where(item => recipesData.ContainsKey(item.Code)).Select(item => recipesData[item.Code]).First();
            recipeItems = recipe.Keys.Select(itemCode => itemsData[itemCode]);
        }

        public static async Task<Dictionary<string, List<List<string>>>> RequestItem(IEnumerable<Item> items, IEnumerable<string> inputQualities, bool filterIsOn = false)
        {   

            Dictionary<string, string> nameDict = items.Select(item => item).ToDictionary(item => item.Code, item => $"{NamesRegex().Replace(item.Name, "")} {item.Tier}");

            string apiUrl = $"https://west.albion-online-data.com/api/v2/stats/prices/{string.Join(",", nameDict.Keys)}";

            if(inputQualities.Any())
            {
                apiUrl += $"?qualities={string.Join(',', qualitiesCode.Where(x => inputQualities.Contains(x.Key)).Select(x => x.Value))}";
            }

            HttpClient client = new();

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();
            
                string responseBody = await response.Content.ReadAsStringAsync();

                IEnumerable<MarketData> list = JsonConvert.DeserializeObject<IEnumerable<MarketData>>(responseBody)!;

                DateTime now = DateTime.UtcNow;
                IEnumerable<MarketData> result;

                if (filterIsOn)
                {
                    result = from item in list
                    where cities.Contains(item.City) && item.SellPriceMin != 0
                    orderby item.SellPriceMin ascending
                    select item;     

                } else {
                    result = from item in list
                    where cities.Contains(item.City)
                    orderby item.SellPriceMin ascending
                    select item; 
                }

                Dictionary<string, List<List<string>>> data = [];

                foreach (var item in result) {
                    if(!data.ContainsKey(item.ItemId)){
                        data[item.ItemId] = [[], [], [], []];
                    }
                    
                    data[item.ItemId][0].Add(item.SellPriceMin.ToString());
                    data[item.ItemId][1].Add($"{qualitiesName[item.Quality]}");
                    data[item.ItemId][2].Add(item.City);
                    data[item.ItemId][3].Add($"há {(now - item.SellPriceMinDate).Hours:D2}:{(now - item.SellPriceMinDate).Minutes:D2} atrás");
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
        [GeneratedRegex(@"\sdo\s(Novato|Iniciante|Adepto|Perito|Mestre|Grão-mestre|Ancião)")]
        private static partial Regex NamesRegex();
        [GeneratedRegex(@"(\d.\d)")]
        private static partial Regex TierRegex();
        [GeneratedRegex(@"(Normal|Bom|Exepcional|Excelente|Obra-Prima)", RegexOptions.IgnoreCase)]
        private static partial Regex QualityRegex();

    }
}