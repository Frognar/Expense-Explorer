using DotMaybe;

namespace ExpenseExplorer.Application.Domain.ValueObjects;

public readonly record struct Category
{
    public string Name { get; }

    private Category(string name) => Name = name;

    public static Maybe<Category> TryCreate(string name) =>
        string.IsNullOrWhiteSpace(name)
            ? None.OfType<Category>()
            : Some.With(new Category(name.Trim()));
}