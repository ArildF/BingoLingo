using System.Diagnostics;
using BingoLingo.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BingoLingo.Server.Admin
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IMongoDatabase _database;

        public AdminController(IMongoDatabase database)
        {
            _database = database;
        }

        [HttpPost]
        public async Task<IActionResult> RefreshDatabase()
        {
            var sw = Stopwatch.StartNew();

            var sessions = await _database.Collection<Session>().AsQueryable().ToListAsync();
            foreach (var session in sessions)
            {
                await _database.Collection<Session>().ReplaceOneAsync(s =>
                    s.Id == session.Id, session);
            }

            var elapsedSessions = sw.Elapsed;
            sw.Restart();

            var translations = await _database.Collection<Translation>().AsQueryable().ToListAsync();
            foreach (var translation in translations)
            {
                await _database.Collection<Translation>().ReplaceOneAsync(t =>
                    t.Id == translation.Id, translation);
            }


            return Ok(
                new
                {
                    Sessions = new
                    {
                        Status = "Ok",
                        Count = sessions.Count,
                        Elapsed = elapsedSessions,
                    },
                    Translations = new
                    {
                        Status = "Ok",
                        Count = translations.Count,
                        Elapsed = sw.Elapsed,
                    },
                });
        }

        [HttpPost]
        public Task TestAuthorization()
        {
            return Task.FromResult(Ok());
        }
    }
}
