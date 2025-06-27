using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;
using static ExpenseExplorer.Application.Helpers.Splitter;

namespace ExpenseExplorer.Application.Features.Categories.GetCategories;

public sealed class GetCategoriesCommandHandler(IGetCategoriesPersistence persistence)
    : IQueryHandler<GetCategoriesRequest, GetCategoriesResponse>
{
    public async Task<Result<GetCategoriesResponse>> HandleAsync(
        GetCategoriesRequest query,
        CancellationToken cancellationToken) =>
        await persistence.GetCategoriesAsync(SplitToUpper(query.SearchTerm), cancellationToken)
            .MapAsync(s => new GetCategoriesResponse(s));
}