using DotMaybe;
using DotResult;
using ExpenseExplorer.Domain.Extensions;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using Mediator;

namespace ExpenseExplorer.Application.Receipts;

public static class CreateReceipt
{
  public sealed record Command(string Store, DateOnly PurchaseDate)
    : ICommand<Result<ReceiptType>>;

  public sealed class Handler(IFactStore<ReceiptType> factStore, TimeProvider timeProvider)
    : ICommandHandler<Command, Result<ReceiptType>>
  {
    public async ValueTask<Result<ReceiptType>> Handle(
      Command command,
      CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(command);
      return await
        from receipt in Validator.Validate(command, DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime))
        from savedReceipt in factStore.SaveAsync(receipt.Id.Value, receipt, cancellationToken)
        select savedReceipt;
    }
  }

  private static class Validator
  {
    public static Result<ReceiptType> Validate(Command command, DateOnly today)
    {
      Maybe<ReceiptType> receipt =
        from store in Store.Create(command.Store)
        from purchaseDate in NonFutureDate.Create(command.PurchaseDate, today)
        select Receipt.Create(store, purchaseDate);

      return receipt.ToResult(() => Failure.Validation(message: "Cannot create receipt"));
    }
  }
}
