using BingoLingo.Shared;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Put(Translation translation)
    {
        translation.Original = translation.Original.Trim();
        translation.Translated = translation.Translated.Trim();

        var coll = _database.GetCollection<Translation>("Translation");
        await coll.InsertOneAsync(translation);

        return Ok();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Post(Translation translation)
    {
        var coll = _database.GetCollection<Translation>("Translation");
        await coll.ReplaceOneAsync(t => t.Id == translation.Id, translation);

        return Ok();
    }

    [Authorize]
    [HttpPost("Search")]
    public async Task<IActionResult> Search(TranslationSearchRequest request)
    {
        int count = await _database.Collection<Translation>().AsQueryable().CountAsync();
        var translations = await _database.Collection<Translation>().AsQueryable()
            .OrderByDescending(t => t.Created)
            .Skip(request.Skip)
            .Take(request.Top)
            .ToListAsync();

        return Ok(new TranslationSearchResponse(count, translations));

    }
}