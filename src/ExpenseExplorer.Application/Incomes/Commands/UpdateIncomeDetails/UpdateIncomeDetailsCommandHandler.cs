namespace ExpenseExplorer.Application.Incomes.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Incomes.Persistence;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Domain.Incomes;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;

public sealed class UpdateIncomeDetailsCommandHandler(IIncomeRepository incomeRepository)
  : ICommandHandler<UpdateIncomeDetailsCommand, Result<Income>>
{
  private readonly IIncomeRepository _incomeRepository = incomeRepository;

  public async Task<Result<Income>> HandleAsync(UpdateIncomeDetailsCommand command, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    return await (
      from incomeId in Id.TryCreate(command.IncomeId).ToResult(() => CommonFailures.InvalidIncomeId)
      from income in _incomeRepository.GetAsync(incomeId, cancellationToken)
      select Update(income, command));
  }

  private static Income Update(Income income, UpdateIncomeDetailsCommand command)
  {
    return income with
    {
      Source = Source.TryCreate(command.Source ?? string.Empty).OrElse(() => income.Source),
      Amount = Money.TryCreate(command.Amount ?? -1).OrElse(() => income.Amount),
      Category = Category.TryCreate(command.Category ?? string.Empty).OrElse(() => income.Category),
      ReceivedDate = command.ReceivedDate.HasValue
        ? NonFutureDate.TryCreate(command.ReceivedDate.Value, command.Today).OrElse(() => income.ReceivedDate)
        : income.ReceivedDate,
      Description = command.Description is not null
        ? Description.TryCreate(command.Description).OrElse(() => income.Description)
        : income.Description,
    };
  }
}
