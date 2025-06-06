using System.Collections.Immutable;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Data;

public interface ICategoryRepository
{
    Task<ImmutableArray<Category>> GetCategoriesAsync(Option<string> search, CancellationToken cancellationToken);
}