using System.Collections.Immutable;
using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Queries;

public sealed record GetStoresQuery(Option<string> SearchTerm);

public sealed class GetStoresHandler(IReceiptRepository receiptRepository)
{
    public async Task<ImmutableArray<Store>> HandleAsync(GetStoresQuery query, CancellationToken cancellationToken) =>
        await receiptRepository.GetStoresAsync(query.SearchTerm, cancellationToken);
}