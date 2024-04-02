namespace ExpenseExplorer.API.Queries;

using CommandHub.Queries;
using ExpenseExplorer.API.Contract;

public class GetReceiptQueryHandler : IQueryHandler<GetReceiptQuery, GetReceiptsResponse>
{
  public Task<GetReceiptsResponse> HandleAsync(GetReceiptQuery query, CancellationToken cancellationToken = default)
  {
    var receipts = Enumerable.Range(0, 10).Select(_ => (ReceiptResponse)null!);
    var response = new GetReceiptsResponse(receipts, 15);
    return Task.FromResult(response);
  }
}
