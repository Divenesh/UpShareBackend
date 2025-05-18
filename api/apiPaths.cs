namespace MyProject.Api;


public class apiPaths
{   
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
        Console.WriteLine("The id is: ",id);
        return item ?? new Dictionary<string, object>();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to get item by ID: {ex.Message}");
        Console.WriteLine("The id is: ",id);
        return new Dictionary<string, object>();
    }
}
}