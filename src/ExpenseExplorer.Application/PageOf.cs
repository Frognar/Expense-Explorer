using System.Collections.Immutable;

namespace ExpenseExplorer.Application;

public sealed record PageOf<T>(ImmutableArray<T> Items, uint TotalCount);

public static class Page
{
    public static PageOf<T> Of<T>(IEnumerable<T> items, uint totalCount) => new([..items], totalCount);
}