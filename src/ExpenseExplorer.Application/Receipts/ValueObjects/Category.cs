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