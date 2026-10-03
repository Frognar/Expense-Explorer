using ExpenseExplorer.Domain.Budget;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Application.Budget;

// Commands carry only domain types, so a command that exists is already valid.

/// <summary>A missing start means "right after the last period".</summary>
public sealed record CreatePeriod(DateOnly? Start, DateOnly? End);

public sealed record Fund(BudgetName Name, FundAmount Amount);

public sealed record PlanItem(Guid GroupId, BudgetName Name, Money Amount);

public sealed record TemplateItem(BudgetName Group, BudgetName Name, Money Amount);

public sealed record Template(IReadOnlyList<Fund> Funds, IReadOnlyList<TemplateItem> Items);

public sealed record Group(BudgetName Name, int Position);

public sealed record CategoryGroup(CategoryName Category, Guid? GroupId);
