using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

public partial class Program
{
    public static void Main(string[] args)
    {
        Server.Run(args);
    }
}



// using MyProject.Database;
// using MyProject.Api;
//
// // Declaration of variables
// var items = new List<Dictionary<string, object>>();
// var filteredItems = new List<Dictionary<string, object>>();

// Create objects
// var db = new GetDatabase();
//
// items = await db.getItemsListed();

// Sorting
// Console.WriteLine("Choose Your Category: ");
// var category = Console.ReadLine();
// filteredItems = apiPaths.GetCategories(items, category);
//
// for (int i = 0; i < filteredItems.Count; i++)
// {
//     Console.WriteLine(filteredItems[i]["name"]);
// }