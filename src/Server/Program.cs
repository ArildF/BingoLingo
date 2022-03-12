using System.Text;
using AspNetCore.Identity.Mongo;
using BingoLingo.Server;
using BingoLingo.Server.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.secret.json", true);


builder.Services.Configure<InitialUserConfig>(builder.Configuration.GetSection("initialUser"));

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtIssuer"],
                ValidAudience = builder.Configuration["JwtAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    builder.Configuration["JwtSecurityKey"]
                ))
            };
        }
    );

builder.Services.AddScoped<InitialUserCreator>();



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
app.UseAuthentication();
app.UseAuthorization();

await app.EnsureInitialUser();

app.Run();