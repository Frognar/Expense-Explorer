using System.Collections.Immutable;
using ExpenseExplorer.Application.Receipts.Data;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Queries;

public sealed record GetCategoriesQuery(Option<string> SearchTerm);

public sealed class GetCategoriesHandler(ICategoryRepository categoryRepository)
{
    public async Task<ImmutableArray<Category>> HandleAsync(GetCategoriesQuery query, CancellationToken cancellationToken) =>
        await categoryRepository.GetCategoriesAsync(query.SearchTerm, cancellationToken);
}