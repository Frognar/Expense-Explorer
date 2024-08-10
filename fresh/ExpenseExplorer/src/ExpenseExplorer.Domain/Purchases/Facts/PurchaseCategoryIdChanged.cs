using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Purchases.Facts;

public record PurchaseCategoryIdChanged(string PurchaseId, string CategoryId) : Fact
{
  public static PurchaseCategoryIdChanged Create(PurchaseIdType purchaseId, ExpenseCategoryIdType categoryId)
    => new(purchaseId.Value, categoryId.Value);
}
