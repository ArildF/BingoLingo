using MongoDB.Bson.Serialization.Attributes;

namespace BingoLingo.Shared;

public class Translation
{
    public string Original { get; set; }
    public string Translated { get; set; }

    [BsonId]
    public string Id { get; set; }
    public Translation(string original, string translated, string id)
    {
        Original = original;
        Translated = translated;
        Id = id;
    }
}