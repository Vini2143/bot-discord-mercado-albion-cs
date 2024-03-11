using Newtonsoft.Json;

namespace Bot.Items {

    
    public class Item
    {
        public string name { get; set; }
        public string code { get; set; }
        public string tier { get; set; }
    }
    
    public class ItemsData
    {
        private static ItemsData? instance;
        private List<Item> data;

        private ItemsData() {
            string json = File.ReadAllText("./src/data/ItemsData.json");
            data = JsonConvert.DeserializeObject<List<Item>>(json);

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

        public List<Item> GetData() {
            return data;
        }

        
    }
} 
