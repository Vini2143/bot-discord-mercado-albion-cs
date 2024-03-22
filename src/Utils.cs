using System.Text.RegularExpressions;
using Bot.Items;
using Newtonsoft.Json;

namespace Bot.Utils
{   
    public class MarketData 
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
    public partial class Functions
    {
        public static IEnumerable<Item> SearchItem(IEnumerable<Item> items, string input)
        {
            IEnumerable<string> tiers = TierRegex().Matches(input).Select(match => match.Value);

            input = CharRegex().Replace(input, "");
            input = input.Trim();

            if (tiers.Any())
            {
                IEnumerable<Item> result = from item in items
                    where item.Name.Contains(input, StringComparison.OrdinalIgnoreCase) && tiers.Contains(item.Tier)
                    select item;
                return result;
            }
            else
            {
                IEnumerable<Item> result = from item in items
                    where item.Name.Contains(input, StringComparison.OrdinalIgnoreCase) 
                    select item;
                return result;
            }
        }

        public static async Task<Dictionary<string, List<string>>> RequestItem(IEnumerable<Item> items)
        {   

            Dictionary<string, string> codeDict = items.Select(item => item).ToDictionary(item => item.Code, item => $"{NamesRegex().Replace(item.Name, "")} {item.Tier}");

            string apiUrl = $"https://west.albion-online-data.com/api/v2/stats/prices/{string.Join(",", codeDict.Keys)}";

            HttpClient client = new();

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();
            
                string responseBody = await response.Content.ReadAsStringAsync();

                IEnumerable<MarketData> list = JsonConvert.DeserializeObject<IEnumerable<MarketData>>(responseBody)!;

                DateTime now = DateTime.UtcNow;

                var result = from item in list
                    where (now - item.SellPriceMinDate).TotalHours <= 12
                    select item;                   
                
                Dictionary<string, List<string>> data = new()
                {
                    ["name"] = [],
                    ["city"] = [],
                    ["price-time"] = [],
                };

                foreach (var item in result) {
                    data["name"].Add(codeDict[item.ItemId]);
                    data["city"].Add(item.City);
                    data["price-time"].Add($"{item.SellPriceMin} há {(now - item.SellPriceMinDate).Hours} horas e {(now - item.SellPriceMinDate).Minutes} minutos");
                }

                return data;
        
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro: " + ex.Message);
                return [];
            }
        }
        
        [GeneratedRegex(@"\d.\d")]
        private static partial Regex TierRegex();
        [GeneratedRegex(@"[^\p{L}\s]")]
        private static partial Regex CharRegex();
        [GeneratedRegex(@"\sdo\s(Novato|Iniciante|Adepto|Perito|Mestre|Grão-mestre|Ancião)")]
        private static partial Regex NamesRegex();
    }
        
        
}