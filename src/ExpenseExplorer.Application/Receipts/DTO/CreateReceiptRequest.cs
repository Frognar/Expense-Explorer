using ExpenseExplorer.Application.Receipts.ValueObjects;

namespace ExpenseExplorer.Application.Receipts.DTO;
using CreateReceiptRequestFactory = Func<Store, PurchaseDate, CreateReceiptRequest>;

public sealed record CreateReceiptRequest(Store Store, PurchaseDate PurchaseDate)
{
    public static readonly CreateReceiptRequestFactory Create = (s, p) => new CreateReceiptRequest(s, p);
}