using DotMaybe;

namespace ExpenseExplorer.Application.Domain.ValueObjects;

public readonly record struct Item
{
    public string Name { get; }

    private Item(string name) => Name = name;

    public static Maybe<Item> TryCreate(string name) =>
        string.IsNullOrWhiteSpace(name)
            ? None.OfType<Item>()
            : Some.With(new Item(name.Trim()));
}