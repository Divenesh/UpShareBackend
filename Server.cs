using MyProject.Api;
using MyProject.Database;

public static class Server
{
    public static void Run(string[] args)
    {
        
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();
        var items = new List<Dictionary<string, object>>();
        var db = new GetDatabase();
        items = db.getItemsListed().Result;
        
        app.MapGet("/home", async (HttpContext context) =>
        {
            await context.Response.WriteAsJsonAsync(items);
        });

        app.MapGet("/home/{category}", async (HttpContext context) =>
        {
            var category = context.Request.RouteValues["category"]?.ToString() ?? string.Empty;
            var filteredItems = apiPaths.GetCategories(items, category);
            await context.Response.WriteAsJsonAsync(filteredItems);
        });

        app.MapGet("/home/item/{id}", async (HttpContext context) =>
        {
            var id = context.Request.RouteValues["id"]?.ToString() ?? string.Empty;
            if (!Guid.TryParse(id, out Guid guidId))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid GUID format");
                return;
            }
            var foundItem = apiPaths.GetItemById(items, guidId);
            List<Dictionary<string, object>> itemList = null;
            if (foundItem != null)
            {
                itemList = new List<Dictionary<string, object>> { foundItem };
            }
            else
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Item not found");
                return;
            }

            var sellerId = itemList?[0]?["sellerId"]?.ToString() ?? string.Empty;
            var seller = apiPaths.GetSeller(items, sellerId);


            var rating = apiPaths.GetRatings(items, id);

            var specification = apiPaths.GetSpecification(items, id);

            await context.Response.WriteAsJsonAsync(new { Items = itemList, Seller = seller, Ratings = rating, Specifications = specification });
        });

        app.Run();
    }
}