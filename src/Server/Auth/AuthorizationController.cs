using BingoLingo.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BingoLingo.Server.Auth;

[ApiController]
[Route("[controller]/[action]")]
public class AuthorizationController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthorizationController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<LoginResult> Login([FromBody] LoginData loginData)
    {
        var signInResult = await _signInManager.PasswordSignInAsync(
                    loginData.UserName,
                    loginData.Password, true, false);
        
        return new LoginResult(signInResult.Succeeded);

    }
}