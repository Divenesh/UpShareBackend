using MyProject.Api;
using MyProject.Database;
using UpShareBackend.centralizedApiRouting;

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
            var filteredItems = RouteInfoProvider.GetInfoForCategories(items, category);
            await context.Response.WriteAsJsonAsync(filteredItems);
        });

        app.MapGet("/home/item/{id}", async (HttpContext context) =>
        {
            var id = context.Request.RouteValues["id"]?.ToString() ?? string.Empty;
            var itemInfo = RouteInfoProvider.GetInfoForItemById(items, Guid.Parse(id));
            await context.Response.WriteAsJsonAsync(itemInfo);
        });
        app.Run();
    }
}