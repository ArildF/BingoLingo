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
            Started = DateTimeOffset.UtcNow,
        };
        await _mongoDatabase.Collection<Session>().InsertOneAsync(session);

        var collection = _mongoDatabase.GetCollection<Translation>("Translation");
        var translations = await collection.AsQueryable().Sample(100).ToListAsync();
        return Ok(new StartedSession(session.Id, translations.ToArray()));
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> SubmitAnswer(string id, SubmittedAnswerRequest answerRequest)
    {
        var session = await _mongoDatabase.Collection<Session>().AsQueryable()
            .Where(s => s.Id == id).FirstOrDefaultAsync();

        if (session == null)
        {
            return NotFound();
        }

        var success = answerRequest.Translation.Translated == answerRequest.Answer;
        session.Results!.Add(new TranslationResult(answerRequest.Translation, answerRequest.Answer, success, DateTimeOffset.UtcNow));

        await _mongoDatabase.Collection<Session>().ReplaceOneAsync(s => s.Id == id, session);

        return Ok(new SubmittedAnswerResponse(success));
    }
}

public class Session
{
    [BsonId]
    public string? Id { get; set; }

    public List<TranslationResult>? Results { get; set; } = new();

    public DateTimeOffset Started { get; set; }

    public DateTimeOffset? Ended { get; set; }
}

public record TranslationResult(Translation Translation, string SubmittedAnswer, bool Success, DateTimeOffset Time);