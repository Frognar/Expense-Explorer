using DotMaybe;

namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct Store
{
    public string Name { get; }

    private Store(string name) => Name = name;

    public static Maybe<Store> TryCreate(string name) =>
        string.IsNullOrWhiteSpace(name)
            ? None.OfType<Store>()
            : Some.With(new Store(name.Trim()));
}