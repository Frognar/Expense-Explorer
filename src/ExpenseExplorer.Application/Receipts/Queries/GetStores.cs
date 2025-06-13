using System.Collections.Immutable;
using DotMaybe;
using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Queries;

public sealed record GetStoresQuery(Maybe<string> SearchTerm);

public sealed class GetStoresHandler(IStoreRepository storeRepository)
{
    public async Task<ImmutableArray<Store>> HandleAsync(GetStoresQuery query, CancellationToken cancellationToken) =>
        await storeRepository.GetStoresAsync(query.SearchTerm, cancellationToken);
}