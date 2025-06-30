using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Features.ReceiptItems.GetReceiptItems;

namespace ExpenseExplorer.Application.Features.ReceiptItems;

public static class ReceiptItemsFeatureFactory
{
    public static IQueryHandler<GetReceiptItemsQuery, GetReceiptItemsResponse>
        CreateGetReceiptItemsQueryHandler(IGetReceiptItemsPersistence getItemsPersistence)
        => new GetReceiptItemsQueryHandler(getItemsPersistence);
}