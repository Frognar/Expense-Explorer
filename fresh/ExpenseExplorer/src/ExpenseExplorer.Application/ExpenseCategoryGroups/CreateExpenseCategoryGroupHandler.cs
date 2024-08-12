using DotMaybe;
using DotResult;
using ExpenseExplorer.Domain.ExpenseCategoryGroups;
using ExpenseExplorer.Domain.ValueObjects;
using Mediator;

namespace ExpenseExplorer.Application.ExpenseCategoryGroups;

public sealed class CreateExpenseCategoryGroupHandler(IFactStore<ExpenseCategoryGroupType> factStore)
  : ICommandHandler<CreateExpenseCategoryGroupCommand, Result<ExpenseCategoryGroupType>>
{
  private readonly IFactStore<ExpenseCategoryGroupType> _factStore = factStore;

  public async ValueTask<Result<ExpenseCategoryGroupType>> Handle(CreateExpenseCategoryGroupCommand command, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(command);
    Maybe<ExpenseCategoryGroupType> group =
      from name in Name.Create(command.Name)
      let description = Description.Create(command.Description)
      select ExpenseCategoryGroup.Create(name, description, ExpenseCategoryIds.New());

    Result<ExpenseCategoryGroupType> groupResult = group.Match(
      () => Fail.OfType<ExpenseCategoryGroupType>(Failure.Validation(message: "Cannot create group")),
      Success.From);

    return await groupResult
      .BindAsync(c => _factStore.SaveAsync(c, cancellationToken));
  }
}
