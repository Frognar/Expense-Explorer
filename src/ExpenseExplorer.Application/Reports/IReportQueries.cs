using ExpenseExplorer.Contracts.Reports;

namespace ExpenseExplorer.Application.Reports;

public interface IReportQueries
{
    Task<CategoryReportResponse> CategoriesAsync(DateOnly start, DateOnly end, CancellationToken cancellationToken);

    /// <summary>Spending per category in <paramref name="count"/> calendar months up to the one with <paramref name="lastDay"/>.</summary>
    Task<MonthlyReportResponse> MonthlyAsync(DateOnly lastDay, int count, CancellationToken cancellationToken);
}
