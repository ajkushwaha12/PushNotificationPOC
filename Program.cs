using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PushPoCApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Logging (detailed)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services
builder.Services.AddControllers();
builder.Services.AddSingleton<VapidService>();
builder.Services.AddSingleton<SubscriptionStore>();

var app = builder.Build();

// Serve static files (wwwroot)
app.UseDefaultFiles(); // serve index.html at /
app.UseStaticFiles();

app.MapControllers();

app.Run();
