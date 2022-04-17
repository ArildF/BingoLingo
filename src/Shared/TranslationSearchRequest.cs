using System.Xml.XPath;

namespace BingoLingo.Shared;

public record TranslationSearchRequest(int Top, int Skip, string? SearchText = null, SortPredicate[] Sorts = null);