namespace ExpenseExplorer.ReadModel.Queries;

using System.Data.Common;
using System.Linq.Expressions;
using CommandHub.Queries;
using DotResult;
using ExpenseExplorer.ReadModel.Extensions;
using ExpenseExplorer.ReadModel.Models;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore.Failures;
using Microsoft.EntityFrameworkCore;

public sealed class GetReceiptsQueryHandler(ExpenseExplorerContext context)
  : IQueryHandler<GetReceiptsQuery, Result<PageOf<ReceiptHeaders>>>
{
  private readonly ExpenseExplorerContext _context = context;

  public async Task<Result<PageOf<ReceiptHeaders>>> HandleAsync(
    GetReceiptsQuery query,
    CancellationToken cancellationToken = default)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(query);
      IQueryable<DbReceipt> receiptQuery = _context.Receipts.AsNoTracking();
      int totalCount = await receiptQuery.CountAsync(cancellationToken);
      receiptQuery = receiptQuery
        .Filter(
          PurchaseDateBetween(query.After, query.Before),
          TotalBetween(query.MinTotal, query.MaxTotal),
          StoreContains(query.Search));

      int filteredCount = await receiptQuery.CountAsync(cancellationToken);
      List<ReceiptHeaders> receipts = await receiptQuery
        .OrderByMany(
          Order.By(GetSelector(query.SortBy), Order.GetDirection(query.SortOrder)),
          Order.DescendingBy<DbReceipt>(r => r.Id))
        .GetPage(query.PageNumber, query.PageSize)
        .Select(r => new ReceiptHeaders(r.Id, r.Store, r.PurchaseDate, r.Total))
        .ToListAsync(cancellationToken);

      PageOf<ReceiptHeaders> response = Page.Of(receipts, totalCount, filteredCount, query.PageSize, query.PageNumber);
      return Success.From(response);
    }
    catch (DbException ex)
    {
      return Fail.OfType<PageOf<ReceiptHeaders>>(FailureFactory.Fatal(ex));
    }
  }

  private static Expression<Func<DbReceipt, bool>> PurchaseDateBetween(DateOnly min, DateOnly max)
  {
    return r => r.PurchaseDate >= min && r.PurchaseDate <= max;
  }

  private static Expression<Func<DbReceipt, bool>> TotalBetween(decimal min, decimal max)
  {
    return r => r.Total >= min && r.Total <= max;
  }

  private static Expression<Func<DbReceipt, bool>> StoreContains(string search)
  {
    if (string.IsNullOrWhiteSpace(search))
    {
      return r => true;
    }

    search = search.ToUpperInvariant();

    // EFCore does not support StringComparison and CultureInfo enums in LINQ queries
#pragma warning disable CA1304
#pragma warning disable CA1311
#pragma warning disable CA1862
    return r => r.Store.ToUpper().Contains(search);
#pragma warning restore CA1862
#pragma warning restore CA1311
#pragma warning restore CA1304
  }

  private static Expression<Func<DbReceipt, object>> GetSelector(string orderBy)
  {
    return orderBy.ToUpperInvariant() switch
    {
      "STORE" => receipt => receipt.Store,
      "PURCHASEDATE" => receipt => receipt.PurchaseDate,
      "TOTAL" => receipt => receipt.Total,
      _ => receipt => receipt.PurchaseDate,
    };
  }
}
