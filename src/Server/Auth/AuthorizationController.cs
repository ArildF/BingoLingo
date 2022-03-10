using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BingoLingo.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BingoLingo.Server.Auth;

[ApiController]
[Route("[controller]/[action]")]
public class AuthorizationController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthorizationController(SignInManager<ApplicationUser> signInManager, 
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginData loginData)
    {
        var signInResult = await _signInManager.PasswordSignInAsync(
                    loginData.UserName,
                    loginData.Password, true, false);

        if (!signInResult.Succeeded)
        {
            return Unauthorized();
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, loginData.UserName!)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.Now.AddDays(180);
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtIssuer"],
            audience: _configuration["JwtAudience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );
        
        return Ok(new LoginResult(signInResult.Succeeded, loginData.UserName, new JwtSecurityTokenHandler().WriteToken(token)));

    }
}