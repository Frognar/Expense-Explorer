using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Features.Stores.GetStores;

namespace ExpenseExplorer.Application.Features.Stores;

public static class StoresFeatureFactory
{
    public static IQueryHandler<GetStoresRequest, GetStoresResponse>
        CreateGetStoresCommandHandler(IGetStoresPersistence getStoresPersistence)
        => new GetStoresCommandHandler(getStoresPersistence);
}