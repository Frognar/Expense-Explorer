using System.Collections.Immutable;
using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Queries;

public sealed record GetCategoriesQuery(Option<string> SearchTerm);

public sealed class GetCategoriesHandler(IReceiptRepository receiptRepository)
{
    public async Task<ImmutableArray<Category>> HandleAsync(GetCategoriesQuery query, CancellationToken cancellationToken) =>
        await receiptRepository.GetCategoriesAsync(query.SearchTerm, cancellationToken);
}