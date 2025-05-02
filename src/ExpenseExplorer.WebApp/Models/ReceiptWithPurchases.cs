namespace ExpenseExplorer.WebApp.Models;

internal sealed record ReceiptWithPurchases(
    Guid Id,
    string Store,
    DateOnly PurchaseDate,
    IEnumerable<PurchaseDetails> Purchases)
{
    public decimal TotalCost => Purchases.Sum(p => p.TotalPrice);
}