using System.Collections.Immutable;
using DotResult;

namespace ExpenseExplorer.Application.Features.Reports.GetCategoryReport;

public interface IGetCategoryReportPersistence
{
    Task<Result<ImmutableList<CategoryExpense>>> GetCategoryReportAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken);
}
