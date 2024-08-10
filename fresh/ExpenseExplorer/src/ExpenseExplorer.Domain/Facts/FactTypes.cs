using System.Diagnostics;
using ExpenseExplorer.Domain.ExpenseCategories.Facts;
using ExpenseExplorer.Domain.ExpenseCategoryGroups.Facts;
using ExpenseExplorer.Domain.Purchases.Facts;
using ExpenseExplorer.Domain.Receipts.Facts;

namespace ExpenseExplorer.Domain.Facts;

public static class FactTypes
{
  public const string ExpenseCategoryGroupCreatedFactType
    = nameof(ExpenseCategoryGroupCreated);

  public const string ExpenseCategoryGroupRenamedFactType
    = nameof(ExpenseCategoryGroupRenamed);

  public const string ExpenseCategoryGroupDescriptionChangedFactType
    = nameof(ExpenseCategoryGroupDescriptionChanged);

  public const string ExpenseCategoryGroupDeletedFactType
    = nameof(ExpenseCategoryGroupDeleted);

  public const string ExpenseCategoryGroupExpenseCategoryAddedFactType
    = nameof(ExpenseCategoryGroupExpenseCategoryAdded);

  public const string ExpenseCategoryGroupExpenseCategoryRemovedFactType
    = nameof(ExpenseCategoryGroupExpenseCategoryRemoved);

  public const string ExpenseCategoryCreatedFactType
    = nameof(ExpenseCategoryCreated);

  public const string ExpenseCategoryRenamedFactType
    = nameof(ExpenseCategoryRenamed);

  public const string ExpenseCategoryDescriptionChangedFactType
    = nameof(ExpenseCategoryDescriptionChanged);

  public const string ExpenseCategoryDeletedFactType
    = nameof(ExpenseCategoryDeleted);

  public const string ExpenseCategoryUsageIncreasedFactType
    = nameof(ExpenseCategoryUsageIncreased);

  public const string ExpenseCategoryUsageDecreasedFactType
    = nameof(ExpenseCategoryUsageDecreased);

  public const string ReceiptCreatedFactType
    = nameof(ReceiptCreated);

  public const string ReceiptStoreChangedFactType
    = nameof(ReceiptStoreChanged);

  public const string ReceiptPurchaseDateChangedFactType
    = nameof(ReceiptPurchaseDateChanged);

  public const string ReceiptDeletedFactType
    = nameof(ReceiptDeleted);

  public const string ReceiptPurchaseAddedFactType
    = nameof(ReceiptPurchaseAdded);

  public const string ReceiptPurchaseRemovedFactType
    = nameof(ReceiptPurchaseRemoved);

  public const string PurchaseCreatedFactType
    = nameof(PurchaseCreated);

  public const string PurchaseItemChangedFactType
    = nameof(PurchaseItemChanged);

  public const string PurchaseCategoryIdChangedFactType
    = nameof(PurchaseCategoryIdChanged);

  public const string PurchaseQuantityChangedFactType
    = nameof(PurchaseQuantityChanged);

  public const string PurchaseUnitPriceChangedFactType
    = nameof(PurchaseUnitPriceChanged);

  public const string PurchaseTotalDiscountChangedFactType
    = nameof(PurchaseTotalDiscountChanged);

  public const string PurchaseDescriptionChangedFactType
    = nameof(PurchaseDescriptionChanged);

  public const string PurchaseDeletedFactType
    = nameof(PurchaseDeleted);

  public static string GetFactType(Fact fact)
  {
    return fact switch
    {
      ExpenseCategoryGroupCreated
        => ExpenseCategoryGroupCreatedFactType,
      ExpenseCategoryGroupRenamed
        => ExpenseCategoryGroupRenamedFactType,
      ExpenseCategoryGroupDescriptionChanged
        => ExpenseCategoryGroupDescriptionChangedFactType,
      ExpenseCategoryGroupDeleted
        => ExpenseCategoryGroupDeletedFactType,
      ExpenseCategoryGroupExpenseCategoryAdded
        => ExpenseCategoryGroupExpenseCategoryAddedFactType,
      ExpenseCategoryGroupExpenseCategoryRemoved
        => ExpenseCategoryGroupExpenseCategoryRemovedFactType,
      ExpenseCategoryCreated
        => ExpenseCategoryCreatedFactType,
      ExpenseCategoryRenamed
        => ExpenseCategoryRenamedFactType,
      ExpenseCategoryDescriptionChanged
        => ExpenseCategoryDescriptionChangedFactType,
      ExpenseCategoryDeleted
        => ExpenseCategoryDeletedFactType,
      ExpenseCategoryUsageIncreased
        => ExpenseCategoryUsageIncreasedFactType,
      ExpenseCategoryUsageDecreased
        => ExpenseCategoryUsageDecreasedFactType,
      ReceiptCreated
        => ReceiptCreatedFactType,
      ReceiptStoreChanged
        => ReceiptStoreChangedFactType,
      ReceiptPurchaseDateChanged
        => ReceiptPurchaseDateChangedFactType,
      ReceiptDeleted
        => ReceiptDeletedFactType,
      ReceiptPurchaseAdded
        => ReceiptPurchaseAddedFactType,
      ReceiptPurchaseRemoved
        => ReceiptPurchaseRemovedFactType,
      PurchaseCreated
        => PurchaseCreatedFactType,
      PurchaseItemChanged
        => PurchaseItemChangedFactType,
      PurchaseCategoryIdChanged
        => PurchaseItemChangedFactType,
      PurchaseQuantityChanged
        => PurchaseItemChangedFactType,
      PurchaseUnitPriceChanged
        => PurchaseItemChangedFactType,
      PurchaseTotalDiscountChanged
        => PurchaseItemChangedFactType,
      PurchaseDescriptionChanged
        => PurchaseItemChangedFactType,
      PurchaseDeleted
        => PurchaseItemChangedFactType,
      _ => throw new UnreachableException(),
    };
  }
}
