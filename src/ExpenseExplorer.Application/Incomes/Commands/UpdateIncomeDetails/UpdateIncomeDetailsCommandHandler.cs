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
      from patchModel in UpdateIncomeDetailsValidator.Validate(command)
      from incomeId in Id.TryCreate(command.IncomeId).ToResult(() => CommonFailures.InvalidIncomeId)
      from income in _incomeRepository.GetAsync(incomeId, cancellationToken)
      select Update(income, patchModel));
  }

  private static Income Update(Income income, IncomePatchModel patchModel)
  {
    return income with
    {
      Source = patchModel.Source.OrElse(() => income.Source),
      Amount = patchModel.Amount.OrElse(() => income.Amount),
      Category = patchModel.Category.OrElse(() => income.Category),
      ReceivedDate = patchModel.ReceivedDate.OrElse(() => income.ReceivedDate),
      Description = patchModel.Description.OrElse(() => income.Description),
    };
  }
}
