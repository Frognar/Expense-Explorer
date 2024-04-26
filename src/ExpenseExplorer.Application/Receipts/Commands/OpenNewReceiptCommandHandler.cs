namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using FunctionalCore.Monads;
using FunctionalCore.Validations;

public class OpenNewReceiptCommandHandler(IReceiptRepository receiptRepository)
  : ICommandHandler<OpenNewReceiptCommand, Result<Receipt>>
{
  private readonly IReceiptRepository _receiptRepository = receiptRepository;

  public async Task<Result<Receipt>> HandleAsync(
    OpenNewReceiptCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    Result<Receipt> resultOfReceipt = OpenNewReceiptValidator.Validate(command).ToResult();
    resultOfReceipt = await SaveAsync(resultOfReceipt, cancellationToken);
    return resultOfReceipt;
  }

  private async Task<Result<Receipt>> SaveAsync(
    Result<Receipt> resultOfReceipt,
    CancellationToken cancellationToken)
  {
    return await resultOfReceipt.Match(
      failure => Task.FromResult(Fail.OfType<Receipt>(failure)),
      receipt => SaveAsync(receipt, cancellationToken));
  }

  private async Task<Result<Receipt>> SaveAsync(Receipt receipt, CancellationToken cancellationToken)
  {
    var eitherFailureOrVersion = await _receiptRepository.SaveAsync(receipt, cancellationToken);
    return eitherFailureOrVersion.MapRight(v => receipt.WithVersion(v).ClearChanges()).ToResult();
  }
}
