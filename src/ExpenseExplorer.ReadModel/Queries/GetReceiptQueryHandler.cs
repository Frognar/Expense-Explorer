namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Extensions;
using ExpenseExplorer.ReadModel.Models;

public class GetReceiptQueryHandler : IQueryHandler<GetReceiptQuery, PageOf<ReceiptHeaders>>
{
  public Task<PageOf<ReceiptHeaders>> HandleAsync(GetReceiptQuery query, CancellationToken cancellationToken = default)
  {
    var receipts = Enumerable.Range(0, 10).Select(_ => (ReceiptHeaders)null!);
    var response = Page.Of(receipts, 15);
    return Task.FromResult(response);
  }
}
