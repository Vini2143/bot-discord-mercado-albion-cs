using Bot.Items;
using Newtonsoft.Json;
using Sprache;

namespace Bot.Utils
{   
    public class MarketData 
    {
        [JsonProperty("item_id")]
        public string ItemId { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
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
    public class Functions
    {
        public static IEnumerable<Item> SearchItem(IEnumerable<Item> items, string input, IEnumerable<string> tiers = default!){
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

        public static async Task RequestItem(IEnumerable<Item> items)
        {   

            IEnumerable<string> codeList = items.Select(item => item.Code);

            string apiUrl = $"https://west.albion-online-data.com/api/v2/stats/prices/{string.Join(",", codeList)}";

            HttpClient client = new();

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    IEnumerable<MarketData> list = JsonConvert.DeserializeObject<IEnumerable<MarketData>>(responseBody);

                    var result = from item in list
                        where item.BuyPriceMax != 0 && item.SellPriceMin != 0
                        orderby item.SellPriceMin
                        select $"{item.ItemId} {item.City} {item.SellPriceMin}";
                        
                    foreach(string registro in result)
                    {
                        Console.WriteLine(registro);
                    }
                    
                }
                else
                {
                    Console.WriteLine("Erro ao realizar a requisição: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro: " + ex.Message);
            }
        }
    }
}