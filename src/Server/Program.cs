using System.ComponentModel.DataAnnotations;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using BingoLingo.Server.Auth;
using Microsoft.AspNetCore.ResponseCompression;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var url = new MongoUrl(config["Mongo:ConnectionString"]);
    var client = new MongoClient(url);
    return client.GetDatabase(url.DatabaseName);
});

builder.Services.AddIdentityMongoDbProvider<ApplicationUser>(
    io =>
    {
        io.Password.RequireDigit = false;
        io.Password.RequireLowercase = false;
        io.Password.RequireUppercase = false;
        io.Password.RequiredLength = 6;
        io.Password.RequireNonAlphanumeric = false;
    },
    mio =>
    {
        mio.ConnectionString = builder.Configuration["Mongo:ConnectionString"];
        
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();