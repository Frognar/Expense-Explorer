using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Purchases.Facts;

public record PurchaseDescriptionChanged(string PurchaseId, string Description) : Fact
{
  public static PurchaseDescriptionChanged Create(PurchaseIdType purchaseId, DescriptionType description)
    => new(purchaseId.Value, description.Value);
}
