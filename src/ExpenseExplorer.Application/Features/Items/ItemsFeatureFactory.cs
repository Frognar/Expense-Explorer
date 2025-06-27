using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Features.Items.GetItems;

namespace ExpenseExplorer.Application.Features.Items;

public static class ItemsFeatureFactory
{
    public static IQueryHandler<GetItemsRequest, GetItemsResponse>
        CreateGetItemsCommandHandler(IGetItemsPersistence getItemsPersistence)
        => new GetItemsCommandHandler(getItemsPersistence);
}