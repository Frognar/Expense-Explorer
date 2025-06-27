using DotMaybe;

namespace ExpenseExplorer.Application.Helpers;

public static class Splitter
{
    public static IEnumerable<string> SplitToUpper(Maybe<string> maybe)
        => maybe
            .Map(s => s.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Map(s => s.Select(str => str.ToUpperInvariant()))
            .Match(() => [], s => s);
}