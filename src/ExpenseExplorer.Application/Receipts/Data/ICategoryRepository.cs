using System.Collections.Immutable;
using DotMaybe;
using ExpenseExplorer.Application.Domain.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Data;

public interface ICategoryRepository
{
    Task<ImmutableArray<Category>> GetCategoriesAsync(Maybe<string> search, CancellationToken cancellationToken);
}