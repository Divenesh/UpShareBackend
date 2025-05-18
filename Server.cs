var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Divenesh Shamugam");
app.MapGet("/ramya", () => "Ramya Ramu");

app.Run();
