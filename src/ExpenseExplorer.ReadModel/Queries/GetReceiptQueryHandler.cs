using System.Linq.Expressions;
using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Models;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore.Failures;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.ReadModel.Queries;

public sealed class GetReceiptQueryHandler(ExpenseExplorerContext context)
  : IQueryHandler<GetReceiptQuery, Result<Receipt>>
{
  private static readonly Expression<Func<DbReceipt, Receipt>> _receiptSelector = r =>
    new Receipt(
      r.Id,
      r.Store,
      r.PurchaseDate,
      r.Total,
      r.Purchases.Select(
        p =>
          new Purchase(p.PurchaseId, p.Item, p.Category, p.Quantity, p.UnitPrice, p.TotalDiscount, p.Description)));

  private readonly ExpenseExplorerContext _context = context;

  public async Task<Result<Receipt>> HandleAsync(
    GetReceiptQuery query,
    CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(query);
    Receipt? receipt = await _context.Receipts
      .Where(r => r.Id == query.ReceiptId)
      .Select(_receiptSelector)
      .FirstOrDefaultAsync(cancellationToken: cancellationToken);

    return receipt is not null
      ? Success.From(receipt)
      : Fail.OfType<Receipt>(FailureFactory.NotFound("Receipt not found.", query.ReceiptId));
  }
}
