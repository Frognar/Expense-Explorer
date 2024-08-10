using System.Diagnostics;
using System.Text.Json;
using ExpenseExplorer.Domain.ExpenseCategories.Facts;
using ExpenseExplorer.Domain.ExpenseCategoryGroups.Facts;
using ExpenseExplorer.Domain.Purchases.Facts;
using ExpenseExplorer.Domain.Receipts.Facts;

namespace ExpenseExplorer.Domain.Facts;

public static class FactSerializer
{
  public static byte[] Serialize(Fact fact)
  {
    return fact switch
    {
      ExpenseCategoryGroupCreated expenseCategoryGroupCreated => Serialize(expenseCategoryGroupCreated),
      ExpenseCategoryGroupRenamed expenseCategoryGroupRenamed => Serialize(expenseCategoryGroupRenamed),
      ExpenseCategoryGroupDescriptionChanged expenseCategoryGroupDescriptionChanged => Serialize(expenseCategoryGroupDescriptionChanged),
      ExpenseCategoryGroupDeleted expenseCategoryGroupDeleted => Serialize(expenseCategoryGroupDeleted),
      ExpenseCategoryGroupExpenseCategoryAdded expenseCategoryGroupExpenseCategoryAdded => Serialize(expenseCategoryGroupExpenseCategoryAdded),
      ExpenseCategoryGroupExpenseCategoryRemoved expenseCategoryGroupExpenseCategoryRemoved => Serialize(expenseCategoryGroupExpenseCategoryRemoved),
      ExpenseCategoryCreated expenseCategoryCreated => Serialize(expenseCategoryCreated),
      ExpenseCategoryRenamed expenseCategoryRenamed => Serialize(expenseCategoryRenamed),
      ExpenseCategoryDescriptionChanged expenseCategoryDescriptionChanged => Serialize(expenseCategoryDescriptionChanged),
      ExpenseCategoryDeleted expenseCategoryDeleted => Serialize(expenseCategoryDeleted),
      ExpenseCategoryUsageIncreased expenseCategoryUsageIncreased => Serialize(expenseCategoryUsageIncreased),
      ExpenseCategoryUsageDecreased expenseCategoryUsageDecreased => Serialize(expenseCategoryUsageDecreased),
      ReceiptCreated receiptCreated => Serialize(receiptCreated),
      ReceiptStoreChanged receiptStoreChanged => Serialize(receiptStoreChanged),
      ReceiptPurchaseDateChanged receiptPurchaseDateChanged => Serialize(receiptPurchaseDateChanged),
      ReceiptDeleted receiptDeleted => Serialize(receiptDeleted),
      ReceiptPurchaseAdded receiptPurchaseAdded => Serialize(receiptPurchaseAdded),
      ReceiptPurchaseRemoved receiptPurchaseRemoved => Serialize(receiptPurchaseRemoved),
      PurchaseCreated purchaseCreated => Serialize(purchaseCreated),
      PurchaseItemChanged itemChanged => Serialize(itemChanged),
      PurchaseCategoryIdChanged categoryIdChanged => Serialize(categoryIdChanged),
      PurchaseQuantityChanged quantityChanged => Serialize(quantityChanged),
      PurchaseUnitPriceChanged unitPriceChanged => Serialize(unitPriceChanged),
      PurchaseTotalDiscountChanged totalDiscountChanged => Serialize(totalDiscountChanged),
      PurchaseDescriptionChanged descriptionChanged => Serialize(descriptionChanged),
      PurchaseDeleted purchaseDeleted => Serialize(purchaseDeleted),
      _ => throw new UnreachableException(),
    };
  }

  public static Fact Deserialize(string type, byte[] data)
  {
    return type switch
    {
      FactTypes.ExpenseCategoryGroupCreatedFactType => Deserialize<ExpenseCategoryGroupCreated>(data),
      FactTypes.ExpenseCategoryGroupRenamedFactType => Deserialize<ExpenseCategoryGroupRenamed>(data),
      FactTypes.ExpenseCategoryGroupDescriptionChangedFactType => Deserialize<ExpenseCategoryGroupDescriptionChanged>(data),
      FactTypes.ExpenseCategoryGroupDeletedFactType => Deserialize<ExpenseCategoryGroupDeleted>(data),
      FactTypes.ExpenseCategoryGroupExpenseCategoryAddedFactType => Deserialize<ExpenseCategoryGroupExpenseCategoryAdded>(data),
      FactTypes.ExpenseCategoryGroupExpenseCategoryRemovedFactType => Deserialize<ExpenseCategoryGroupExpenseCategoryRemoved>(data),
      FactTypes.ExpenseCategoryCreatedFactType => Deserialize<ExpenseCategoryCreated>(data),
      FactTypes.ExpenseCategoryRenamedFactType => Deserialize<ExpenseCategoryRenamed>(data),
      FactTypes.ExpenseCategoryDescriptionChangedFactType => Deserialize<ExpenseCategoryDescriptionChanged>(data),
      FactTypes.ExpenseCategoryDeletedFactType => Deserialize<ExpenseCategoryDeleted>(data),
      FactTypes.ExpenseCategoryUsageIncreasedFactType => Deserialize<ExpenseCategoryUsageIncreased>(data),
      FactTypes.ExpenseCategoryUsageDecreasedFactType => Deserialize<ExpenseCategoryUsageDecreased>(data),
      FactTypes.ReceiptCreatedFactType => Deserialize<ReceiptCreated>(data),
      FactTypes.ReceiptStoreChangedFactType => Deserialize<ReceiptStoreChanged>(data),
      FactTypes.ReceiptPurchaseDateChangedFactType => Deserialize<ReceiptPurchaseDateChanged>(data),
      FactTypes.ReceiptDeletedFactType => Deserialize<ReceiptDeleted>(data),
      FactTypes.ReceiptPurchaseAddedFactType => Deserialize<ReceiptPurchaseAdded>(data),
      FactTypes.ReceiptPurchaseRemovedFactType => Deserialize<ReceiptPurchaseRemoved>(data),
      FactTypes.PurchaseCreatedFactType => Deserialize<PurchaseCreated>(data),
      FactTypes.PurchaseItemChangedFactType => Deserialize<PurchaseItemChanged>(data),
      FactTypes.PurchaseCategoryIdChangedFactType => Deserialize<PurchaseCategoryIdChanged>(data),
      FactTypes.PurchaseQuantityChangedFactType => Deserialize<PurchaseQuantityChanged>(data),
      FactTypes.PurchaseUnitPriceChangedFactType => Deserialize<PurchaseUnitPriceChanged>(data),
      FactTypes.PurchaseTotalDiscountChangedFactType => Deserialize<PurchaseTotalDiscountChanged>(data),
      FactTypes.PurchaseDescriptionChangedFactType => Deserialize<PurchaseDescriptionChanged>(data),
      FactTypes.PurchaseDeletedFactType => Deserialize<PurchaseDeleted>(data),
      _ => throw new UnreachableException(),
    };
  }

  private static byte[] Serialize<T>(T fact)
    where T : Fact
  {
    return JsonSerializer.SerializeToUtf8Bytes(fact);
  }

  private static T Deserialize<T>(byte[] data)
  {
    return JsonSerializer.Deserialize<T>(data)!;
  }
}
