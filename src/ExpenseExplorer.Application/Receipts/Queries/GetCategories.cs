using System.Collections.Immutable;
using DotMaybe;
using ExpenseExplorer.Application.Domain.ValueObjects;
using ExpenseExplorer.Application.Receipts.Data;

namespace ExpenseExplorer.Application.Receipts.Queries;

public sealed record GetCategoriesQuery(Maybe<string> SearchTerm);

public sealed class GetCategoriesHandler(ICategoryRepository categoryRepository)
{
    public async Task<ImmutableArray<Category>> HandleAsync(GetCategoriesQuery query, CancellationToken cancellationToken) =>
        await categoryRepository.GetCategoriesAsync(query.SearchTerm, cancellationToken);
}