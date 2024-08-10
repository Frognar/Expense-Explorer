using System.Diagnostics;
using ExpenseExplorer.Domain.ExpenseCategoryGroups.Facts;
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
  public const string ExpenseCategoryGroupCreatedFactType = nameof(ExpenseCategoryGroupCreated);
  public const string ExpenseCategoryGroupRenamedFactType = nameof(ExpenseCategoryGroupRenamed);
  public const string ExpenseCategoryGroupDescriptionChangedFactType = nameof(ExpenseCategoryGroupDescriptionChanged);
  public const string ExpenseCategoryGroupDeletedFactType = nameof(ExpenseCategoryGroupDeleted);
  public const string ExpenseCategoryGroupExpenseCategoryAddedFactType = nameof(ExpenseCategoryGroupExpenseCategoryAdded);
  public const string ExpenseCategoryGroupExpenseCategoryRemovedFactType = nameof(ExpenseCategoryGroupExpenseCategoryRemoved);

  public static string GetFactType(Fact fact)
  {
    return fact switch
    {
      ExpenseCategoryGroupCreated => ExpenseCategoryGroupCreatedFactType,
      ExpenseCategoryGroupRenamed => ExpenseCategoryGroupRenamedFactType,
      ExpenseCategoryGroupDescriptionChanged => ExpenseCategoryGroupDescriptionChangedFactType,
      ExpenseCategoryGroupDeleted => ExpenseCategoryGroupDeletedFactType,
      ExpenseCategoryGroupExpenseCategoryAdded => ExpenseCategoryGroupExpenseCategoryAddedFactType,
      ExpenseCategoryGroupExpenseCategoryRemoved => ExpenseCategoryGroupExpenseCategoryRemovedFactType,
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
