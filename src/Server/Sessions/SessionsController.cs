using BingoLingo.Shared;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BingoLingo.Server.Sessions;

[ApiController]
[Route("[controller]")]
public class SessionsController : Controller
{
    private readonly IMongoDatabase _mongoDatabase;

    public SessionsController(IMongoDatabase mongoDatabase)
    {
        _mongoDatabase = mongoDatabase;
    }

    [HttpPut]
    public async Task<IActionResult> NewSession()
    {
        var session = new Session
        {
            Id = Guid.NewGuid().ToString(),
            Results = new TranslationResult[]{}
        };
        await _mongoDatabase.GetCollection<Session>("Session").InsertOneAsync(session);

        var collection = _mongoDatabase.GetCollection<Translation>("Translation");
        var translations = await collection.AsQueryable().Sample(100).ToListAsync();
        return Ok(new StartedSession(session.Id, translations.ToArray()));

    }
}

public class Session
{
    [BsonId]
    public string Id { get; set; }
    public TranslationResult[] Results { get; set; }
}

public record TranslationResult(Translation Translation, string SubmittedAnswer, bool Success);