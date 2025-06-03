using System.Collections.Immutable;
using System.Diagnostics;

namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct Item
{
    public string Name { get; }

    private Item(string name) => Name = name;

    public static Option<Item> TryCreate(string name) =>
        string.IsNullOrWhiteSpace(name)
            ? Option.None<Item>()
            : Option.Some(new Item(name.Trim()));
}

public static class Items
{
    public static Option<ImmutableArray<Item>> CreateMany(params IEnumerable<Option<Item>> items) =>
        items.Any(o => o.IsNone)
            ? Option.None<ImmutableArray<Item>>()
            : Option.Some<ImmutableArray<Item>>([
                ..items.Select(o => o.OrElse(() => throw new UnreachableException()))
            ]);
}