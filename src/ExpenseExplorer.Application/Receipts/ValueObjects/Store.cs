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