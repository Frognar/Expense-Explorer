using DotResult;

namespace ExpenseExplorer.Application.Features.Stores.GetStores;

public interface IGetStoresPersistence
{
    Task<Result<IEnumerable<string>>> GetStoresAsync(
        IEnumerable<string> searchTerms,
        CancellationToken cancellationToken);
}