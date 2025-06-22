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

        app.MapGet(
            "/home",
            async (HttpContext context) =>
            {
                await context.Response.WriteAsJsonAsync(items);
            }
        );

        app.MapGet(
            "/home/{category}",
            async (HttpContext context) =>
            {
                var category = context.Request.RouteValues["category"]?.ToString() ?? string.Empty;
                var filteredItems = RouteInfoProvider.GetInfoForCategories(items, category);
                await context.Response.WriteAsJsonAsync(filteredItems);
            }
        );

        app.MapGet(
            "/home/item/{id}",
            async (HttpContext context) =>
            {
                var id = context.Request.RouteValues["id"]?.ToString() ?? string.Empty;
                var itemInfo = RouteInfoProvider.GetInfoForItemById(items, Guid.Parse(id));
                await context.Response.WriteAsJsonAsync(itemInfo);
            }
        );

        app.MapGet(
            "/user/{userId}",
            async (HttpContext context) =>
            {
                var userId = context.Request.RouteValues["userId"]?.ToString() ?? string.Empty;
                var userInfo = await RouteInfoProvider.GetUser(userId);
                await context.Response.WriteAsJsonAsync(userInfo);
            }
        );

        app.MapPost(
            "/user/",
            async (HttpContext context) =>
            {
                // Change this to explicitly request a Dictionary
                var user = await context.Request.ReadFromJsonAsync<Dictionary<string, object>>();

                if (user != null)
                {
                    var result = await RouteInfoProvider.CreateUser(user);
                    await context.Response.WriteAsJsonAsync(result);
                }
                else
                {
                    context.Response.StatusCode = 400; // Bad Request
                    await context.Response.WriteAsync("Invalid user data.");
                }
            }
        );
        app.Run();
    }
}
