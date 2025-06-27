using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;
using static ExpenseExplorer.Application.Helpers.Splitter;

namespace ExpenseExplorer.Application.Features.Stores.GetStores;

internal sealed class GetStoresCommandHandler(IGetStoresPersistence persistence)
    : IQueryHandler<GetStoresRequest, GetStoresResponse>
{
    public async Task<Result<GetStoresResponse>> HandleAsync(
        GetStoresRequest query,
        CancellationToken cancellationToken) =>
        await persistence.GetStoresAsync(SplitToUpper(query.SearchTerm), cancellationToken)
            .MapAsync(s => new GetStoresResponse(s));
}