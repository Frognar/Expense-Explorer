using DotResult;
using ExpenseExplorer.Domain.ExpenseCategoryGroups;
using Mediator;

namespace ExpenseExplorer.Application.ExpenseCategoryGroups;

public sealed record CreateExpenseCategoryGroupCommand(string Name, string? Description) : ICommand<Result<ExpenseCategoryGroupType>>;
