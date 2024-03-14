using Bot.Items;

namespace Bot.Utils
{
    public class Functions
    {
        public static IEnumerable<string> SearchItem(List<Item> items, string input, IEnumerable<string> tiers = default!){
            if (tiers.Any())
            {
                IEnumerable<string> result = from item in items
                    where item.Name.Contains(input, StringComparison.OrdinalIgnoreCase) && tiers.Contains(item.Tier)
                    select $"{item.Name} {item.Tier}";
                return result;
            }
            else
            {
                IEnumerable<string> result = from item in items
                    where item.Name.Contains(input, StringComparison.OrdinalIgnoreCase) 
                    select $"{item.Name} {item.Tier}";
                return result;
            }
        }
    }
}