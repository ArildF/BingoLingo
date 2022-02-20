using BingoLingo.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BingoLingo.Server.User;

[ApiController]
[Route("[controller]")]
public class UserController : Controller
{
    // GET
    [HttpGet("")]
    public async Task<IActionResult> GetUser()
    {
        return Ok(new UserDetails(HttpContext.User?.Identity?.Name));
    }
}