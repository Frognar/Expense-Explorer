using DotResult;
using ExpenseExplorer.Domain.ExpenseCategoryGroups;
using ExpenseExplorer.Domain.Extensions;
using ExpenseExplorer.Domain.ValueObjects;
using Mediator;

namespace ExpenseExplorer.Application.ExpenseCategoryGroups;

public static class RenameExpenseCategoryGroup
{
  public sealed record Command(string Id, string Name)
    : ICommand<Result<ExpenseCategoryGroupType>>;

  public sealed class Handler(IFactStore<ExpenseCategoryGroupType> factStore)
    : ICommandHandler<Command, Result<ExpenseCategoryGroupType>>
  {
    public async ValueTask<Result<ExpenseCategoryGroupType>> Handle(
      Command command,
      CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(command);
      Result<ExpenseCategoryGroupType> loadedGroup = await factStore.ReadAsync(command.Id, cancellationToken);
      return await Rename(loadedGroup, command)
        .BindAsync(g => factStore.SaveAsync(g.Id.Value, g, cancellationToken));
    }

    private static Result<ExpenseCategoryGroupType> Rename(
      Result<ExpenseCategoryGroupType> loadedGroup,
      Command command)
      =>
        from name in Validator.Validate(command)
        from @group in loadedGroup
        from renamed in @group.Rename(name)
        select renamed;
  }

  private static class Validator
  {
    public static Result<NameType> Validate(Command command)
      => Name.Create(command.Name)
        .ToResult(() => Failure.Validation(message: "Wrong name"));
  }
}
