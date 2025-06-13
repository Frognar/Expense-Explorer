using System.Collections.Immutable;
using DotMaybe;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Data;

public interface IItemRepository
{
    Task<ImmutableArray<Item>> GetItemsAsync(Maybe<string> search, CancellationToken cancellationToken);
}