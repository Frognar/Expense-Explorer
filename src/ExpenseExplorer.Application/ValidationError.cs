namespace ExpenseExplorer.Application;

public sealed record ValidationError(string PropertyName, string Error);