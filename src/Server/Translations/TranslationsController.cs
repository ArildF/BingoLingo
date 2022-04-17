using System.ComponentModel;
using BingoLingo.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BingoLingo.Server.Translations;

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
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var coll = _database.Collection<Translation>();
        await coll.DeleteOneAsync(t => t.Id == id);

        return Ok();
    }

    [Authorize]
    [HttpPost("Search")]
    public async Task<IActionResult> Search(TranslationSearchRequest request)
    {
        var queryable = _database.Collection<Translation>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            queryable = queryable.Where(q =>
                q.Original.ToLowerInvariant().Contains(request.SearchText.ToLowerInvariant()) || 
                q.Translated.ToLowerInvariant().Contains(request.SearchText.ToLowerInvariant()));
        }
        int count = await queryable.CountAsync();

        if (!request.Sorts.EmptyIfNull().Any())
        {
            queryable = queryable.OrderByDescending(t => t.Created);
        }
        else
        {
            foreach (var sort in request.Sorts.EmptyIfNull())
            {
                queryable = sort switch
                {
                    { Property: "Created", Direction: ListSortDirection.Ascending } =>
                        queryable.OrderBy(t => t.Created),
                    { Property: "Created", Direction: ListSortDirection.Descending } => queryable.OrderByDescending(t =>
                        t.Created),
                    { Property: "Modified", Direction: ListSortDirection.Ascending } => queryable.OrderBy(t =>
                        t.Modified),
                    { Property: "Modified", Direction: ListSortDirection.Descending } => queryable.OrderByDescending(
                        t =>
                            t.Modified),
                    { Property: "Original", Direction: ListSortDirection.Ascending } => queryable.OrderBy(t =>
                        t.Original),
                    { Property: "Original", Direction: ListSortDirection.Descending } => queryable.OrderByDescending(
                        t =>
                            t.Original),
                    { Property: "Translated", Direction: ListSortDirection.Ascending } => queryable.OrderBy(t =>
                        t.Translated),
                    { Property: "Translated", Direction: ListSortDirection.Descending } => queryable.OrderByDescending(
                        t =>
                            t.Translated),
                    _ => queryable
                };

            }
        }

        var translations = await queryable
            .Skip(request.Skip)
            .Take(request.Top)
            .ToListAsync();

        return Ok(new TranslationSearchResponse(count, translations));

    }
}