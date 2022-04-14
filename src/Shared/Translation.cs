using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BingoLingo.Shared;

public class Translation
{
    public string Original { get; set; }
    public string Translated { get; set; }

    [BsonRepresentation(BsonType.Document)]
    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

    [BsonRepresentation(BsonType.Document)]
    public DateTimeOffset? Modified { get; set; }

    [BsonId]
    public string Id { get; set; }
    public Translation(string original, string translated, string id)
    {
        Original = original;
        Translated = translated;
        Id = id;
    }
}