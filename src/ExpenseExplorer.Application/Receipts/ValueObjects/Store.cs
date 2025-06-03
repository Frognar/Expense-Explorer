using System.Collections.Immutable;
using System.Diagnostics;

namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct Store
{
    public string Name { get; }

    private Store(string name) => Name = name;

    public static Option<Store> TryCreate(string name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? Option.None<Store>()
            : Option.Some(new Store(name.Trim()));
    }
}

public static class Stores
{
    public static Option<ImmutableArray<Store>> CreateMany(params IEnumerable<Option<Store>> stores) =>
        stores.Any(o => o.IsNone)
            ? Option.None<ImmutableArray<Store>>()
            : Option.Some<ImmutableArray<Store>>([
                ..stores.Select(o => o.OrElse(() => throw new UnreachableException()))
            ]);
}