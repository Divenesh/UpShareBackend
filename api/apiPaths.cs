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
}