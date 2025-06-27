using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;
using static ExpenseExplorer.Application.Helpers.Splitter;

namespace ExpenseExplorer.Application.Features.Items.GetItems;

internal sealed class GetItemsCommandHandler(IGetItemsPersistence persistence)
    : IQueryHandler<GetItemsRequest, GetItemsResponse>
{
    public async Task<Result<GetItemsResponse>> HandleAsync(
        GetItemsRequest query,
        CancellationToken cancellationToken) =>
        await persistence.GetItemsAsync(SplitToUpper(query.SearchTerm), cancellationToken)
            .MapAsync(s => new GetItemsResponse(s));
}