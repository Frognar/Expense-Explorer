namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using FunctionalCore.Monads;

public class OpenNewReceiptCommandHandler(IReceiptRepository receiptRepository)
  : ICommandHandler<OpenNewReceiptCommand, Result<Receipt>>
{
  private readonly IReceiptRepository _receiptRepository = receiptRepository;

  public async Task<Result<Receipt>> HandleAsync(
    OpenNewReceiptCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    return
      await (
        from receipt in OpenNewReceiptValidator.Validate(command)
        from version in _receiptRepository.SaveAsync(receipt, cancellationToken)
        select receipt.WithVersion(version).ClearChanges());
  }
}
