using System.Xml.XPath;

namespace BingoLingo.Shared;

public record TranslationSearchRequest(int Top, int Skip, SortPredicate[] Sorts = null);