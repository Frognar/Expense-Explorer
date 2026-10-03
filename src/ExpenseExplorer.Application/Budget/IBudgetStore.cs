using ExpenseExplorer.Domain.Budget;

namespace ExpenseExplorer.Application.Budget;

/// <summary>
/// Storage of budgets. Methods that change one thing answer <c>false</c> or <c>null</c> when
/// that thing does not exist, and leave turning it into an error to the use cases.
/// </summary>
public interface IBudgetStore
{
    Task<IReadOnlyList<(Guid Id, BudgetPeriod Period)>> PeriodsAsync(CancellationToken cancellationToken);

    /// <summary>Creates the period with copies of the template's incomes and planned expenses.</summary>
    Task<Guid> CreatePeriodAsync(BudgetPeriod period, CancellationToken cancellationToken);

    Task<bool> DeletePeriodAsync(Guid periodId, CancellationToken cancellationToken);

    Task<Guid?> AddFundAsync(Guid periodId, Fund fund, CancellationToken cancellationToken);

    Task<bool> ChangeFundAsync(Guid periodId, Guid fundId, Fund fund, CancellationToken cancellationToken);

    Task<bool> RemoveFundAsync(Guid periodId, Guid fundId, CancellationToken cancellationToken);

    Task<bool> PeriodExistsAsync(Guid periodId, CancellationToken cancellationToken);

    Task<bool> GroupExistsAsync(Guid groupId, CancellationToken cancellationToken);

    Task<Guid> AddItemAsync(Guid periodId, PlanItem item, CancellationToken cancellationToken);

    Task<bool> ChangeItemAsync(Guid periodId, Guid itemId, PlanItem item, CancellationToken cancellationToken);

    Task<bool> RemoveItemAsync(Guid periodId, Guid itemId, CancellationToken cancellationToken);

    Task<bool> GroupNameTakenAsync(BudgetName name, Guid? exceptGroupId, CancellationToken cancellationToken);

    Task<Guid> AddGroupAsync(Group group, CancellationToken cancellationToken);

    Task<bool> ChangeGroupAsync(Guid groupId, Group group, CancellationToken cancellationToken);

    Task<bool> GroupInUseAsync(Guid groupId, CancellationToken cancellationToken);

    Task<bool> DeleteGroupAsync(Guid groupId, CancellationToken cancellationToken);

    Task SetCategoryGroupAsync(CategoryGroup assignment, CancellationToken cancellationToken);

    /// <summary>Replaces the template and creates the groups it names that do not exist yet.</summary>
    Task ReplaceTemplateAsync(Template template, CancellationToken cancellationToken);
}
