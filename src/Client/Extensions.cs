using System.ComponentModel;
using BingoLingo.Shared;
using Radzen;

namespace BingoLingo.Client;
public static class Extensions
{
    public static SortPredicate ToSortPredicate(this SortDescriptor self)
    {
        return new SortPredicate(
            self.SortOrder switch
            {
                SortOrder.Ascending => ListSortDirection.Ascending,
                SortOrder.Descending => ListSortDirection.Descending,
                _ => throw new ArgumentOutOfRangeException(nameof(self.SortOrder)),
            }, self.Property);

    }
}
