using System.Collections.Immutable;
using System.Diagnostics;

namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct Category
{
    public string Name { get; }

    private Category(string name) => Name = name;

    public static Option<Category> TryCreate(string name) =>
        string.IsNullOrWhiteSpace(name)
            ? Option.None<Category>()
            : Option.Some(new Category(name.Trim()));
}

public static class Categories
{
    public static Option<ImmutableArray<Category>> CreateMany(params IEnumerable<Option<Category>> categories) =>
        categories.Any(o => o.IsNone)
            ? Option.None<ImmutableArray<Category>>()
            : Option.Some<ImmutableArray<Category>>([
                ..categories.Select(o => o.OrElse(() => throw new UnreachableException()))
            ]);
}