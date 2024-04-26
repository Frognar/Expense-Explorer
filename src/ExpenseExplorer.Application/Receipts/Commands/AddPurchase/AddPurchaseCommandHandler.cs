namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;
using FunctionalCore.Validations;

public class AddPurchaseCommandHandler(IReceiptRepository receiptRepository)
  : ICommandHandler<AddPurchaseCommand, Result<Receipt>>
{
  private readonly IReceiptRepository _receiptRepository = receiptRepository;

  public async Task<Result<Receipt>> HandleAsync(
    AddPurchaseCommand command,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(command);
    Result<Purchase> resultOfPurchase = AddPurchaseValidator.Validate(command).ToResult();
    Result<Receipt> resultOfReceipt = await AddPurchaseAsync(resultOfPurchase, command.ReceiptId, cancellationToken);
    return await SaveAsync(resultOfReceipt, cancellationToken);
  }

  private async Task<Result<Receipt>> AddPurchaseAsync(
    Result<Purchase> resultOfPurchase,
    string receiptId,
    CancellationToken cancellationToken)
  {
    return await Id.TryCreate(receiptId)
      .Match(
        () => Task.FromResult(Fail.OfType<Receipt>(CommonFailures.InvalidReceiptId)),
        async id => await AddPurchaseAsync(resultOfPurchase, id, cancellationToken));
  }

  private async Task<Result<Receipt>> AddPurchaseAsync(
    Result<Purchase> resultOfPurchase,
    Id receiptId,
    CancellationToken cancellationToken)
  {
    return await resultOfPurchase.Match(
      failure => Task.FromResult(Fail.OfType<Receipt>(failure)),
      purchase => AddPurchaseAsync(purchase, receiptId, cancellationToken));
  }

  private async Task<Result<Receipt>> AddPurchaseAsync(
    Purchase purchase,
    Id receiptId,
    CancellationToken cancellationToken)
  {
    Result<Receipt> resultOfReceipt = await _receiptRepository.GetAsync(receiptId, cancellationToken);
    return resultOfReceipt.Map(r => r.AddPurchase(purchase));
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
    Result<Version> resultOfVersion = await _receiptRepository.SaveAsync(receipt, cancellationToken);
    return resultOfVersion.Map(v => receipt.WithVersion(v).ClearChanges());
  }
}
