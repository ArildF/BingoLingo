using BingoLingo.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BingoLingo.Server.Controllers;

[ApiController]
public class TranslationsController : Controller
{
    // GET
    [Route("randomTranslation")]
    public IActionResult RandomTranslation()
    {
        return Ok(new Translation("No", "Non"));
    }
}