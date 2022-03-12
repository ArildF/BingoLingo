using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BingoLingo.Server.Auth;

public class InitialUserCreator
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOptions<InitialUserConfig> _config;

    public InitialUserCreator(UserManager<ApplicationUser> userManager, 
        IOptions<InitialUserConfig> config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task CreateInitialUser()
    {
        var hasAdmin = _userManager.Users.Any(u => u.UserName == _config.Value.UserName);
        if (!hasAdmin)
        {
            await _userManager.CreateAsync(new ApplicationUser
            {
                UserName = _config.Value.UserName
            }, _config.Value.Password);
        }
    }
}