namespace ExpenseExplorer.Contracts.Reports;

/// <summary>Spending per category between two dates (both included), largest first.</summary>
public sealed record CategoryReportResponse(
    DateOnly From,
    DateOnly To,
    IReadOnlyList<CategoryExpenseResponse> Categories,
    decimal Total);

public sealed record CategoryExpenseResponse(string Category, decimal Total);
