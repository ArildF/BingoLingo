using BingoLingo.Shared;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BingoLingo.Server.Sessions;

[ApiController]
[Route("[controller]")]
public class SessionsController : Controller
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly AnswerComparer _comparer;

    public SessionsController(IMongoDatabase mongoDatabase, AnswerComparer comparer)
    {
        _mongoDatabase = mongoDatabase;
        _comparer = comparer;
    }

    [HttpGet("{userId}/User")]
    public async Task<IActionResult> SessionsForUser(string userId)
    {
        var sessions = await _mongoDatabase.Collection<Session>()
            .AsQueryable().Where(s => s.UserId == userId)
            .OrderByDescending(s => s.Started)
            .ToListAsync();
        return Ok(sessions);
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> NewSession(string userId, [FromQuery]int numQuestions=5)
    {
        var session = new Session
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Started = DateTimeOffset.UtcNow,
        };
        await _mongoDatabase.Collection<Session>().InsertOneAsync(session);

        var collection = _mongoDatabase.GetCollection<Translation>("Translation");
        var translations = await collection.AsQueryable().Sample(numQuestions).ToListAsync();
        return Ok(new StartedSession(session.Id, translations.ToArray()));
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> SubmitAnswer(string id, SubmittedAnswerRequest answerRequest)
    {
        var session = await GetSession(id);

        if (session == null)
        {
            return NotFound();
        }


        var success = _comparer.IsAcceptable(
	        answerRequest.Translation.Translated.Trim(),
	        answerRequest.Answer.Trim());
        session.Results!.Add(new TranslationResult(answerRequest.Translation, answerRequest.Answer, success, DateTimeOffset.UtcNow));

        await StoreSession(id, session);

        return Ok(new SubmittedAnswerResponse(success));
    }

    [HttpPost("{id}/completed")]
    public async Task<IActionResult> SessionCompleted(string id)
    {
        var session = await GetSession(id);

        if (session == null)
        {
            return NotFound();
        }
        
        session.Ended = DateTimeOffset.UtcNow;

        await StoreSession(id, session);

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Session(string id)
    {
        var session = await GetSession(id);
        if (session == null)
        {
            return NotFound();
        }

        return Ok(session);
    }

    private async Task<Session?> GetSession(string id)
    {
        var session = await _mongoDatabase.Collection<Session>().AsQueryable()
            .Where(s => s.Id == id).FirstOrDefaultAsync();
        return session;
    }

    private async Task StoreSession(string id, Session session)
    {
        await _mongoDatabase.Collection<Session>().ReplaceOneAsync(s => s.Id == id, session);
    }
}