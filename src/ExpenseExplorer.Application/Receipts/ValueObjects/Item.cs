namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct Item
{
    public string Name { get; }
    
    private Item(string name) => Name = name;

    public static Result<Item, string> TryCreate(string name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? Result.Failure<Item, string>("Item name cannot be empty")
            : Result.Success<Item, string>(new Item(name.Trim()));
    }
}