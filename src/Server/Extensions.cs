using MongoDB.Driver;

namespace BingoLingo.Server;

public static class Extensions
{
    public static IMongoCollection<T> Collection<T>(this IMongoDatabase self)
    {
        return self.GetCollection<T>(typeof(T).Name);
    }
    
}