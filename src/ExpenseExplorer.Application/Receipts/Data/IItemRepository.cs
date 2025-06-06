using System.Collections.Immutable;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Data;

public interface IItemRepository
{
    Task<ImmutableArray<Item>> GetItemsAsync(Option<string> search, CancellationToken cancellationToken);
}