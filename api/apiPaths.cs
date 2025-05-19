using System.Data.Common;
using MyProject.Database;

namespace MyProject.Api;


public class apiPaths
{
    public static GetDatabase db = new GetDatabase();
    public static List<Dictionary<string, object>> GetCategories(List<Dictionary<string, object>> items, string category)
    {

        try
        {
            return items.Where(item => item["category"].ToString() == category).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to filter items: {ex.Message}");
            return items;
        }
    }
    public static Dictionary<string, object> GetItemById(List<Dictionary<string, object>> items, Guid id)
    {
        try
        {
            var item = items.FirstOrDefault(item => (Guid)item["id"] == id);
            Console.WriteLine("The id is: ", id);
            return item ?? new Dictionary<string, object>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to get item by ID: {ex.Message}");
            Console.WriteLine("The id is: ", id);
            return new Dictionary<string, object>();
        }
    }
    public static List<Dictionary<string, object>> GetSeller(List<Dictionary<string, object>> items, string sellerId)
    {
        try
        {
            var seller = new List<Dictionary<string, object>>();
            seller = db.getSellers().Result;
            var sellerItem = seller.FirstOrDefault(item => item["id"].ToString() == sellerId);

            Console.WriteLine("The sellerItem: " + sellerItem["sellername"]);
            return sellerItem != null ? new List<Dictionary<string, object>> { sellerItem } : new List<Dictionary<string, object>>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to filter items by seller ID: {ex.Message}");
            return items;
        }
    }

    public static List<Dictionary<string, object>> GetRatings(List<Dictionary<string, object>> items, string itemId)
    {
        try
        {
            var ratings = new List<Dictionary<string, object>>();
            ratings = db.getRatings().Result;
            var ratingItems = ratings.Where(rating => rating["itemid"].ToString() == itemId);
            // System.Console.WriteLine("Rating items: " + string.Join(", ", ratingItems.Select(r => r["rating"])));
            return ratingItems.Any() ? ratingItems.ToList() : new List<Dictionary<string, object>>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to filter items by item ID: {ex.Message}");
            return items;
        }
    }

    public static List<Dictionary<string, object>> GetSpecification(List<Dictionary<string, object>> items, string itemId)
    {
        try
        {
            var specifications = new List<Dictionary<string, object>>();
            specifications = db.getSpecification().Result;
            var specificationItems = specifications.Where(specification => specification["itemid"].ToString() == itemId);
            // System.Console.WriteLine("Specification items: " + string.Join(", ", specificationItems.Select(r => r["details"])));
            return specificationItems.Any() ? specificationItems.ToList() : new List<Dictionary<string, object>>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to filter specification items by item ID: {ex.Message}");
            return items;
        }
    }

}