namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct Item
{
    public string Name { get; }

    private Item(string name) => Name = name;

    public static Option<Item> TryCreate(string name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? Option.None<Item>()
            : Option.Some(new Item(name.Trim()));
    }
}