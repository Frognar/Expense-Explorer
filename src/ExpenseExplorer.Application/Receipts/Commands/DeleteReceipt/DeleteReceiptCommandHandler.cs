using CommandHub.Commands;
using DotResult;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore;

namespace ExpenseExplorer.Application.Receipts.Commands;

public sealed class DeleteReceiptCommandHandler(IReceiptRepository receiptRepository)
  : ICommandHandler<DeleteReceiptCommand, Result<Unit>>
{
  private readonly IReceiptRepository _receiptRepository = receiptRepository;

  public async Task<Result<Unit>> HandleAsync(
    DeleteReceiptCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    return await (
      from receiptId in Id.TryCreate(command.ReceiptId).ToResult(() => CommonFailures.InvalidReceiptId)
      from receipt in _receiptRepository.GetAsync(receiptId, cancellationToken)
      let deleted = receipt.Delete()
      from version in _receiptRepository.SaveAsync(deleted, cancellationToken)
      select Unit.Instance);
  }
}
