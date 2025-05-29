using ExpenseExplorer.WebApp.Models;

namespace ExpenseExplorer.WebApp.Data;

internal sealed class InMemoryReceiptRepository : IReceiptRepository
{
    private static readonly List<ReceiptWithPurchases> Receipts =
        Enumerable.Range(1, 10000)
            .Select(i => new ReceiptWithPurchases(
                Guid.CreateVersion7(),
                $"Store {i}",
                DateOnly.FromDateTime(DateTime.Today).AddDays(-i),
                []))
            .ToList();

    public async Task AddAsync(ReceiptDetails receipt)
    {
        Receipts.Add(new ReceiptWithPurchases(receipt.Id, receipt.Store, receipt.PurchaseDate, []));
        await Task.CompletedTask;
    }

    public async Task<ReceiptWithPurchases?> GetReceiptAsync(Guid id)
    {
        ReceiptWithPurchases? receipt = Receipts.FirstOrDefault(r => r.Id == id);
        if (receipt == null)
        {
            return null;
        }

        await Task.CompletedTask;
        return receipt;
    }

    public async Task DeleteReceiptAsync(Guid id)
    {
        Receipts.RemoveAll(r => r.Id == id);
        await Task.CompletedTask;
    }

    public Task AddPurchaseAsync(Guid receiptId, PurchaseDetails purchase)
    {
        ReceiptWithPurchases receipt = Receipts.FirstOrDefault(r => r.Id == receiptId)!;
        int index = Receipts.IndexOf(receipt);
        Receipts[index] = receipt with { Purchases = receipt.Purchases.Append(purchase).ToList() };
        return Task.CompletedTask;
    }

    public Task UpdatePurchaseAsync(Guid receiptId, PurchaseDetails purchase)
    {
        ReceiptWithPurchases receipt = Receipts.FirstOrDefault(r => r.Id == receiptId)!;
        int index = Receipts.IndexOf(receipt);
        Receipts[index] = receipt with { Purchases = UpdatePurchase(receipt.Purchases, purchase) };
        return Task.CompletedTask;
    }

    private static IEnumerable<PurchaseDetails> UpdatePurchase(IEnumerable<PurchaseDetails> purchases, PurchaseDetails purchase)
    {
        foreach (PurchaseDetails purchaseDetails in purchases)
        {
            if (purchaseDetails.Id == purchase.Id)
            {
                yield return purchase;
            }
            else
            {
                yield return purchaseDetails;
            }
        }
    }

    public Task DeletePurchaseAsync(Guid receiptId, Guid purchaseId)
    {
        ReceiptWithPurchases receipt = Receipts.FirstOrDefault(r => r.Id == receiptId)!;
        int index = Receipts.IndexOf(receipt);
        Receipts[index] = receipt with { Purchases = receipt.Purchases.Where(p => p.Id != purchaseId).ToList() };
        return Task.CompletedTask;
    }
}