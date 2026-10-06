namespace ExpenseExplorer.Contracts.Reports;

/// <summary>
/// Spending per calendar month, oldest month first. <see cref="Totals"/> and each category's totals
/// have one value per month, in the order of <see cref="Months"/>. Categories come largest first.
/// </summary>
public sealed record MonthlyReportResponse(
    IReadOnlyList<DateOnly> Months,
    IReadOnlyList<decimal> Totals,
    IReadOnlyList<CategoryMonthsResponse> Categories);

public sealed record CategoryMonthsResponse(string Category, IReadOnlyList<decimal> Totals, decimal Total);
