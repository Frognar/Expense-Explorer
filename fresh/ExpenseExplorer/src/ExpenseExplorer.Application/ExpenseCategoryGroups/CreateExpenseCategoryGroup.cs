using DotMaybe;
using DotResult;
using ExpenseExplorer.Domain.ExpenseCategoryGroups;
using ExpenseExplorer.Domain.Extensions;
using ExpenseExplorer.Domain.ValueObjects;
using Mediator;

namespace ExpenseExplorer.Application.ExpenseCategoryGroups;

public static class CreateExpenseCategoryGroup
{
  public sealed record Command(string Name, string? Description)
    : ICommand<Result<ExpenseCategoryGroupType>>;

  public sealed class Handler(IFactStore<ExpenseCategoryGroupType> factStore)
    : ICommandHandler<Command, Result<ExpenseCategoryGroupType>>
  {
    public async ValueTask<Result<ExpenseCategoryGroupType>> Handle(
      Command command,
      CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(command);
      return await Validator.Validate(command)
        .BindAsync(g => factStore.SaveAsync(g.Id.Value, g, cancellationToken));
    }
  }

  private static class Validator
  {
    public static Result<ExpenseCategoryGroupType> Validate(Command command)
    {
      Maybe<ExpenseCategoryGroupType> group =
        from name in Name.Create(command.Name)
        let description = Description.Create(command.Description)
        select ExpenseCategoryGroup.Create(name, description);

      return group.ToResult(() => Failure.Validation(message: "Cannot create group"));
    }
  }
}
