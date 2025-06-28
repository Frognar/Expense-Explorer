namespace ExpenseExplorer.Application.Features.ReceiptItems.GetReceiptItems;

public record GetReceiptItemsResponse(
    PageOf<ReceiptItemDetails> Items,
    decimal Total);