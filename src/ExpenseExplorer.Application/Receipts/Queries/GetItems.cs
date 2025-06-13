using System.Collections.Immutable;
using DotMaybe;
using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Queries;

public sealed record GetItemsQuery(Maybe<string> SearchTerm);

public sealed class GetItemsHandler(IItemRepository itemRepository)
{
    public async Task<ImmutableArray<Item>> HandleAsync(GetItemsQuery query, CancellationToken cancellationToken) =>
        await itemRepository.GetItemsAsync(query.SearchTerm, cancellationToken);
}