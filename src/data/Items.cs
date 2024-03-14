using Newtonsoft.Json;

namespace Bot.Items {

    
    public class Item
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
        public required string Tier { get; set; }
    }
    
    public sealed class ItemsData
    {
        private static ItemsData? instance;
        private IEnumerable<Item> data;

        private ItemsData()
        {
            string json = File.ReadAllText("./src/data/ItemsData.json");
            data = JsonConvert.DeserializeObject<IEnumerable<Item>>(json);

        }

        public static ItemsData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ItemsData();
                }
                return instance;
            }
        }

        public IEnumerable<Item> Data
        {
            get
            {
                return data;
            }
        }
    }
} 
