namespace ExpenseExplorer.Application.Receipts.Commands;

using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Application.Validations;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;

public class AddPurchaseCommandCommandHandler(IReceiptRepository repository)
{
  private readonly IReceiptRepository repository = repository;

  public async Task<Either<Failure, Receipt>> HandleAsync(AddPurchaseCommand command)
  {
    ArgumentNullException.ThrowIfNull(command);
    Validated<Purchase> validated = PurchaseValidator.Validate(command);
    Either<Failure, Purchase> either = validated.ToEither().MapLeft(e => (Failure)e);
    Id receiptId = Id.Create(command.ReceiptId);
    Receipt? receipt = await repository.GetAsync(receiptId);
    if (receipt is null)
    {
      return Left.From<Failure, Receipt>(new NotFoundFailure("Receipt not found", receiptId));
    }

    return either.MapRight(_ => receipt);
  }
}
