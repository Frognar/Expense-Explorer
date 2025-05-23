using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.DTO;

using CreateReceiptRequestFactory = Func<Store, PurchaseDate, CreateReceiptRequest>;

public sealed record CreateReceiptRequest(ReceiptId Id, Store Store, PurchaseDate PurchaseDate)
{
    public static readonly CreateReceiptRequestFactory Create = (s, p) =>
        new CreateReceiptRequest(ReceiptId.Unique(), s, p);
}