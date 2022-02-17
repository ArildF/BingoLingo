namespace BingoLingo.Shared;

public record TranslationResult(Translation Translation, string SubmittedAnswer, bool Success, DateTimeOffset Time);