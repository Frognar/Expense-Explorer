using System.Collections.Immutable;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Data;

public interface IStoreRepository
{
    Task<ImmutableArray<Store>> GetStoresAsync(Option<string> search, CancellationToken cancellationToken);
}