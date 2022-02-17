using MongoDB.Bson.Serialization.Attributes;

namespace BingoLingo.Shared;

public class Session
{
    [BsonId]
    public string? Id { get; set; }

    public List<TranslationResult> Results { get; set; } = new();

    public DateTimeOffset Started { get; set; }

    public DateTimeOffset? Ended { get; set; }
}