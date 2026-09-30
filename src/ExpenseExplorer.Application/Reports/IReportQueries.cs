using ExpenseExplorer.Contracts.Reports;

namespace ExpenseExplorer.Application.Reports;

public interface IReportQueries
{
    Task<CategoryReportResponse> CategoriesAsync(DateOnly start, DateOnly end, CancellationToken cancellationToken);
}
