namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Models;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore.Failures;
using FunctionalCore.Monads;
using Microsoft.EntityFrameworkCore;

public class GetReceiptQueryHandler(ExpenseExplorerContext context)
  : IQueryHandler<GetReceiptQuery, Either<Failure, Receipt>>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Either<Failure, Receipt>> HandleAsync(
    GetReceiptQuery query,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(query);
    DbReceipt? receipt = await _context.Receipts
      .Include(r => r.Purchases)
      .FirstOrDefaultAsync(r => r.Id == query.ReceiptId, cancellationToken: cancellationToken);

    return receipt is not null
      ? Right.From<Failure, Receipt>(
        new Receipt(
          receipt.Id,
          receipt.Store,
          receipt.PurchaseDate,
          receipt.Total,
          receipt.Purchases.Select(
            p => new Purchase(
              p.PurchaseId,
              p.Item,
              p.Category,
              p.Quantity,
              p.UnitPrice,
              p.TotalDiscount,
              p.Description))))
      : Left.From<Failure, Receipt>(new NotFoundFailure("Receipt not found.", query.ReceiptId));
  }
}
