using System.Collections.Immutable;
using DotMaybe;
using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.Data;

public interface IStoreRepository
{
    Task<ImmutableArray<Store>> GetStoresAsync(Maybe<string> search, CancellationToken cancellationToken);
}