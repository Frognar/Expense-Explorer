using ExpenseExplorer.Contracts.Budget;

namespace ExpenseExplorer.Application.Budget;

public interface IBudgetQueries
{
    Task<IReadOnlyList<PeriodResponse>> PeriodsAsync(CancellationToken cancellationToken);

    /// <summary>The period with spending from receipts so far; <c>null</c> when there is no such period.</summary>
    Task<BudgetResponse?> GetAsync(Guid periodId, DateOnly today, CancellationToken cancellationToken);

    Task<Guid?> PeriodOnAsync(DateOnly day, CancellationToken cancellationToken);

    Task<IReadOnlyList<GroupResponse>> GroupsAsync(CancellationToken cancellationToken);

    Task<TemplateResponse> TemplateAsync(CancellationToken cancellationToken);
}
