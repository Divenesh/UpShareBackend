
using MyProject.Api;

namespace UpShareBackend.centralizedApiRouting;

public class RouteInfoProvider
{
    public static List<Dictionary<string, object>> GetInfoForCategories(List<Dictionary<string, object>> items, string category)
    {
        try
        {
            var filteredItems = apiPaths.GetCategories(items, category);
            return filteredItems;
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Failed to filter items by category: {ex.Message}");
            return items;
        }
    }
    public static List<Dictionary<string, object>> GetInfoForItemById(List<Dictionary<string, object>> items, Guid guid)
    {
        try
        {
            var id = guid.ToString();
            var foundItem = apiPaths.GetItemById(items, id);
            var itemList = foundItem != null 
                ? new List<Dictionary<string, object>> { foundItem }
                : new List<Dictionary<string, object>>();
                
            if (itemList.Count > 0)
            {
                Console.WriteLine("The itemList: " + itemList[0]["name"]);
            }
            
            var sellerId = itemList.Count > 0 ? itemList[0]?["sellerId"]?.ToString() ?? string.Empty : string.Empty;
            var seller = apiPaths.GetSeller(items, sellerId);
            var rating = apiPaths.GetRatings(items, id);
            var specification = apiPaths.GetSpecification(items, id);
            var result = new List<Dictionary<string, object>>();
            if (itemList.Count > 0)
            {
                var itemCollection = new Dictionary<string, object>
                {
                    ["item"] = itemList[0]
                };
                result.Add(itemCollection);
            }
            if (seller.Count > 0)
            {
                var sellerCollection = new Dictionary<string, object>
                {
                    ["sellers"] = seller
                };
                result.Add(sellerCollection);
            }

            // Add all ratings as a named collection
            if (rating.Count > 0)
            {
                var ratingCollection = new Dictionary<string, object>
                {
                    ["ratings"] = rating
                };
                result.Add(ratingCollection);
            }

            // Add all specifications as a named collection
            if (specification.Count > 0)
            {
                var specificationCollection = new Dictionary<string, object>
                {
                    ["specifications"] = specification
                };
                result.Add(specificationCollection);
            }  return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to get item by ID: {ex.Message}");
            return new List<Dictionary<string, object>>();
        }
    }

}
