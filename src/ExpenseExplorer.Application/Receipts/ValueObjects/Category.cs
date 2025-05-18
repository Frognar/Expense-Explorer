namespace ExpenseExplorer.Application.Receipts.ValueObjects;

public readonly record struct Category
{
    public string Name { get; }

    private Category(string name) => Name = name;

    public static Result<Category, string> TryCreate(string name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? Result.Failure<Category, string>("Category name cannot be empty")
            : Result.Success<Category, string>(new Category(name.Trim()));
    }
}