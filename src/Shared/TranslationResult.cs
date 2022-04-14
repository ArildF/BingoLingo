using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BingoLingo.Shared;

public record TranslationResult(Translation Translation, string SubmittedAnswer, bool Success, 
    [property:BsonRepresentation(BsonType.Document)]DateTimeOffset Time);