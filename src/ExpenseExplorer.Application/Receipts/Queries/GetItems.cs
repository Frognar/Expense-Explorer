using System.Collections.Immutable;
using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Queries;

public sealed record GetItemsQuery(Option<string> SearchTerm);

public sealed class GetItemsHandler(IReceiptRepository receiptRepository)
{
    public async Task<ImmutableArray<Item>> HandleAsync(GetItemsQuery query, CancellationToken cancellationToken) =>
        await receiptRepository.GetItemsAsync(query.SearchTerm, cancellationToken);
}