using System.Text.RegularExpressions;
using Bot.Items;
using Newtonsoft.Json;

namespace Bot.Utils
{   
    public class MarketData 
    {
        [JsonProperty("item_id")]
        public string? ItemId { get; set; }
        [JsonProperty("city")]
        public string? City { get; set; }
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

        public static async Task<IEnumerable<string>> RequestItem(IEnumerable<Item> items)
        {   

            Dictionary<string, string> codeDict = items.Select(item => item).ToDictionary(item => item.Code, item => $"{item.Name} {item.Tier}");

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
                    orderby item.SellPriceMin
                    select $"{codeDict[item.ItemId!]} {item.City} {item.SellPriceMin} há {(now - item.SellPriceMinDate).Hours} horas e {(now - item.SellPriceMinDate).Minutes} minutos";
                    
                return result;
        
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
    }
        
        
}