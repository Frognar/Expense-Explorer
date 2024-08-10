using System.Diagnostics;
using ExpenseExplorer.Domain.Purchases.Facts;

namespace ExpenseExplorer.Domain.Facts;

public static class FactTypes
{
  public const string PurchaseCreatedFactType = nameof(PurchaseCreated);
  public const string PurchaseItemChangedFactType = nameof(PurchaseItemChanged);
  public const string PurchaseCategoryIdChangedFactType = nameof(PurchaseCategoryIdChanged);
  public const string PurchaseQuantityChangedFactType = nameof(PurchaseQuantityChanged);
  public const string PurchaseUnitPriceChangedFactType = nameof(PurchaseUnitPriceChanged);
  public const string PurchaseTotalDiscountChangedFactType = nameof(PurchaseTotalDiscountChanged);
  public const string PurchaseDescriptionChangedFactType = nameof(PurchaseDescriptionChanged);
  public const string PurchaseDeletedFactType = nameof(PurchaseDeleted);

  public static string GetFactType(Fact fact)
  {
    return fact switch
    {
      PurchaseCreated => PurchaseCreatedFactType,
      PurchaseItemChanged => PurchaseItemChangedFactType,
      PurchaseCategoryIdChanged => PurchaseItemChangedFactType,
      PurchaseQuantityChanged => PurchaseItemChangedFactType,
      PurchaseUnitPriceChanged => PurchaseItemChangedFactType,
      PurchaseTotalDiscountChanged => PurchaseItemChangedFactType,
      PurchaseDescriptionChanged => PurchaseItemChangedFactType,
      PurchaseDeleted => PurchaseItemChangedFactType,
      _ => throw new UnreachableException(),
    };
  }
}
