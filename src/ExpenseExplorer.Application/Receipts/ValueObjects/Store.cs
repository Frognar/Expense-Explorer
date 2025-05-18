namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct Store
{
    public string Name { get; }
    
    private Store(string name) => Name = name;

    public static Result<Store, string> TryCreate(string name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? Result<Store, string>.Failure("Store name cannot be empty")
            : Result<Store, string>.Success(new Store(name.Trim()));
    }
}