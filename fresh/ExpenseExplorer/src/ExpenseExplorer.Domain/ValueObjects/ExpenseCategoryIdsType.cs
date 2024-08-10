using System.Collections.ObjectModel;

namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct ExpenseCategoryIdsType(IReadOnlyCollection<ExpenseCategoryIdType> Ids);

public static class ExpenseCategoryIds
{
  public static ExpenseCategoryIdsType New(
    params ExpenseCategoryIdType[] ids)
  {
    return new ExpenseCategoryIdsType(new ReadOnlyCollection<ExpenseCategoryIdType>(ids.ToList()));
  }

  public static bool Contains(
    this ExpenseCategoryIdsType ids,
    ExpenseCategoryIdType id)
    => ids.Ids.Contains(id);

  public static ExpenseCategoryIdsType Append(
    this ExpenseCategoryIdsType ids,
    ExpenseCategoryIdType id)
    => ids.Ids.Contains(id)
      ? ids
      : new ExpenseCategoryIdsType(new ReadOnlyCollection<ExpenseCategoryIdType>(ids.Ids.Append(id).ToList()));

  public static ExpenseCategoryIdsType Without(
    this ExpenseCategoryIdsType ids,
    ExpenseCategoryIdType id)
    => new(new ReadOnlyCollection<ExpenseCategoryIdType>(ids.Ids.Where(i => i != id).ToList()));
}
