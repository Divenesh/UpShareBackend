using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MyProject.Database;
using UpShareBackend.centralizedApiRouting;
using UpShareBackend.Controllers;
using UpShareBackend.supabaseAuth;

public static class Server
{
    public static void Run(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder
            .Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline
        app.UseRouting();
        app.UseCors();
        app.UseAuthorization();

        // Serve static files (uploads)
        app.UseStaticFiles();

        app.MapControllers();
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
                if (!context.Request.HasFormContentType)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Content-Type must be multipart/form-data.");
                    return;
                }

                var form = await context.Request.ReadFormAsync();
                var user = new Dictionary<string, object>();

                // Handle file upload
                string profilePictureUrl = "";
                var file = form.Files["profilePicture"];
                if (file != null && file.Length > 0)
                {
                    Console.WriteLine("File received: " + file.FileName);
                    try
                    {
                        var authModel = new AuthModel();
                        var userId = form["id"].ToString();
                        if (!string.IsNullOrEmpty(userId))
                        {
                            profilePictureUrl = await authModel.UpdateUserProfile(file, userId);
                            Console.WriteLine($"File uploaded successfully: {profilePictureUrl}");
                        }
                        else
                        {
                            Console.WriteLine("UserId is required for profile picture upload.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"File upload error: {ex.Message}");
                    }
                }
                else
                {
                    profilePictureUrl = "/assets/images/user.png";
                }
                foreach (var key in form.Keys)
                {
                    if (key != "profilePicture")
                    {
                        user[key] = form[key].ToString();
                    }
                }

                if (!string.IsNullOrEmpty(profilePictureUrl))
                {
                    user["profilePicture"] = profilePictureUrl;
                }

                Console.WriteLine(
                    "Received user data: "
                        + string.Join(", ", user.Select(kv => $"{kv.Key}: {kv.Value}"))
                );

                var result = await RouteInfoProvider.CreateUser(user);
                await context.Response.WriteAsJsonAsync(result ?? new Dictionary<string, object>());
            }
        );

        app.MapPut(
            "/user/",
            async (HttpContext context) =>
            {
                if (!context.Request.HasFormContentType)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Content-Type must be multipart/form-data.");
                    return;
                }

                Console.WriteLine("Updating user profile...");
                var form = await context.Request.ReadFormAsync();
                var user = new Dictionary<string, object>();

                string profilePictureUrl = "";
                var file = form.Files["profilePicture"];
                if (file != null && file.Length > 0)
                {
                    Console.WriteLine("File received: " + file.FileName);
                    try
                    {
                        var authModel = new AuthModel();
                        var userId = form["id"].ToString();
                        if (!string.IsNullOrEmpty(userId))
                        {
                            profilePictureUrl = await authModel.UpdateUserProfile(file, userId);
                            Console.WriteLine($"File uploaded successfully: {profilePictureUrl}");
                        }
                        else
                        {
                            Console.WriteLine("UserId is required for profile picture upload.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"File upload error: {ex.Message}");
                    }
                }
                else
                {
                    profilePictureUrl = "/assets/images/user.png";
                }
                foreach (var key in form.Keys)
                {
                    if (key != "profilePicture")
                    {
                        user[key] = form[key].ToString();
                    }
                }

                if (!string.IsNullOrEmpty(profilePictureUrl))
                {
                    user["profilePicture"] = profilePictureUrl;
                }

                Console.WriteLine(
                    "Received user data: "
                        + string.Join(", ", user.Select(kv => $"{kv.Key}: {kv.Value}"))
                );

                var result = await RouteInfoProvider.UpdateUser(user);
                await context.Response.WriteAsJsonAsync(result ?? new Dictionary<string, object>());
            }
        );

        app.MapGet(
            "/login/auth/signin",
            async (HttpContext context) =>
            {
                var authModel = new AuthModel();
                var email = context.Request.Query["email"].ToString();
                var password = context.Request.Query["password"].ToString();

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Email and password are required.");
                    return;
                }

                try
                {
                    var session = await authModel.SignInAsync(email, password);
                    await context.Response.WriteAsJsonAsync(session);
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync($"SignIn error: {ex.Message}");
                }
            }
        );
        app.Run();
    }
}
