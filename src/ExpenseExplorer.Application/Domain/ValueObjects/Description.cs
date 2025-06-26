namespace ExpenseExplorer.Application.Domain.ValueObjects;

public readonly record struct Description(string? Value)
{
    public string? Value { get; } = Value?.Trim();
}