using DotMaybe;
using DotResult;
using ExpenseExplorer.Domain.ExpenseCategories;
using ExpenseExplorer.Domain.ExpenseCategoryGroups;
using ExpenseExplorer.Domain.Extensions;
using ExpenseExplorer.Domain.ValueObjects;
using Mediator;

namespace ExpenseExplorer.Application.ExpenseCategories;

public static class CreateExpenseCategory
{
  public sealed record Command(string GroupId, string Name, string? Description)
    : ICommand<Result<ExpenseCategoryType>>;

  public sealed class Handler(
    IFactStore<ExpenseCategoryGroupType> groupFactStore,
    IFactStore<ExpenseCategoryType> categoryFactStore)
    : ICommandHandler<Command, Result<ExpenseCategoryType>>
  {
    public async ValueTask<Result<ExpenseCategoryType>> Handle(
      Command command,
      CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(command);
      return await (
        from @group in groupFactStore.ReadAsync(command.GroupId, ExpenseCategoryGroup.Recreate, cancellationToken)
        from category in Validator.Validate(command)
        from savedCategory in categoryFactStore.SaveAsync(category.Id.Value, category, cancellationToken)
        from updatedGroup in @group.AddExpenseCategory(category.Id)
        from savedGroup in groupFactStore.SaveAsync(updatedGroup.Id.Value, updatedGroup, cancellationToken)
        select savedCategory);
    }
  }

  private static class Validator
  {
    public static Result<ExpenseCategoryType> Validate(Command command)
    {
      Maybe<ExpenseCategoryType> category =
        from groupId in ExpenseCategoryGroupId.Create(command.GroupId)
        from name in Name.Create(command.Name)
        let description = Description.Create(command.Description)
        select ExpenseCategory.Create(groupId, name, description);

      return category.ToResult(() => Failure.Validation(message: "Cannot create category"));
    }
  }
}
