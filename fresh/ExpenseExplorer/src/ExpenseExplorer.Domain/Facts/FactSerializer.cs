using System.Diagnostics;
using System.Text.Json;
using ExpenseExplorer.Domain.ExpenseCategoryGroups.Facts;
using ExpenseExplorer.Domain.Purchases.Facts;

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
