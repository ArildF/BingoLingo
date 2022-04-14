using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BingoLingo.Shared;

public class Session
{
    [BsonId]
    public string? Id { get; set; }

    public string? UserId { get; set; }

    public List<TranslationResult> Results { get; set; } = new();

    [BsonRepresentation(BsonType.Document)]
    public DateTimeOffset Started { get; set; }

    [BsonRepresentation(BsonType.Document)]
    public DateTimeOffset? Ended { get; set; }
}