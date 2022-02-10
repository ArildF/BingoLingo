using Microsoft.Extensions.Logging;

namespace BingoLingo.Shared;

public static class Extensions
{
    public static async Task ThrowIfError(this HttpResponseMessage msg) => await ThrowIfError<object>(msg);
    public static async Task ThrowIfError<T>(this HttpResponseMessage msg, ILogger<T>? logger = null)
    {
        if (!msg.IsSuccessStatusCode)
        {
            var response = await msg.Content.ReadAsStringAsync();
            var message = $"Got {msg.StatusCode}: {msg.ReasonPhrase} from {msg.RequestMessage?.RequestUri}. {response}";
            logger?.LogError(message);
            throw new Exception(message);
        }
        
    }
}