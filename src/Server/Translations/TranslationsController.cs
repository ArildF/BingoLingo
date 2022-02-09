using BingoLingo.Shared;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BingoLingo.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class TranslationsController : Controller
{
    private readonly IMongoDatabase _database;

    public TranslationsController(IMongoDatabase database)
    {
        _database = database;
    }

    // GET
    [Route("random")]
    public async Task<IActionResult> RandomTranslation()
    {
        var coll = _database.GetCollection<Translation>("Translation");
        var translation = await coll.AsQueryable().Sample(1).FirstOrDefaultAsync();

        return translation != null ? Ok(translation) : NotFound();
    }

    [HttpPut]
    public async Task<IActionResult> Put(Translation translation)
    {
        translation.Original = translation.Original.Trim();
        translation.Translated = translation.Translated.Trim();

        var coll = _database.GetCollection<Translation>("Translation");
        await coll.InsertOneAsync(translation);

        return Ok();
    }
}