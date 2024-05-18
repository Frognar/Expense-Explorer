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
      let updatedIncome = Update(income, patchModel)
      from version in _incomeRepository.SaveAsync(updatedIncome, cancellationToken)
      select updatedIncome.WithVersion(version).ClearChanges());
  }

  private static Income Update(Income income, IncomePatchModel patchModel)
  {
    var updated0 = patchModel.Source.Match(() => income, income.CorrectSource);
    var updated1 = patchModel.Amount.Match(() => updated0, updated0.CorrectAmount);
    var updated2 = patchModel.Category.Match(() => updated1, updated1.CorrectCategory);
    var updated3 = patchModel.ReceivedDate.Match(() => updated2, rd => updated2.CorrectReceivedDate(rd, patchModel.Today));
    return patchModel.Description.Match(() => updated3, updated3.CorrectDescription);
  }
}
