namespace ExpenseExplorer.Application.Incomes.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Incomes.Persistence;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Domain.Incomes;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore;
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
    return income
      .Apply(i => patchModel.Source.Match(() => i, i.CorrectSource))
      .Apply(i => patchModel.Amount.Match(() => i, i.CorrectAmount))
      .Apply(i => patchModel.Category.Match(() => i, i.CorrectCategory))
      .Apply(i => patchModel.ReceivedDate.Match(() => i, rd => i.CorrectReceivedDate(rd, patchModel.Today)))
      .Apply(i => patchModel.Description.Match(() => i, i.CorrectDescription));
  }
}
