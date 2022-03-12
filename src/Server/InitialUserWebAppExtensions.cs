using BingoLingo.Server.Auth;

namespace BingoLingo.Server;

public static class InitialUserWebAppExtensions
{
    public static async Task<WebApplication> EnsureInitialUser(this WebApplication application)
    {
        using (var scope = application.Services.CreateScope())
        {
            var creator = scope.ServiceProvider.GetRequiredService<InitialUserCreator>();
            await creator.CreateInitialUser();

            return application;
        }
    }
    
}