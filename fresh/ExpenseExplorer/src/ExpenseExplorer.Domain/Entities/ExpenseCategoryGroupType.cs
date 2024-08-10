using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Entities;

public readonly record struct ExpenseCategoryGroupType(
  ExpenseCategoryGroupIdType Id,
  NameType Name,
  DescriptionType Description,
  ExpenseCategoryIdsType ExpenseCategoryIds,
  bool Deleted);

public static class ExpenseCategoryGroup
{
  public static ExpenseCategoryGroupType Create(
    NameType name,
    DescriptionType description,
    ExpenseCategoryIdsType expenseCategoryIds)
  {
    return new ExpenseCategoryGroupType(ExpenseCategoryGroupId.Unique(), name, description, expenseCategoryIds, false);
  }

  public static ExpenseCategoryGroupType Rename(
    this ExpenseCategoryGroupType group,
    NameType newName)
  {
    return group.Name == newName
      ? group
      : group with { Name = newName };
  }

  public static ExpenseCategoryGroupType ChangeDescription(
    this ExpenseCategoryGroupType group,
    DescriptionType newDescription)
  {
    return group.Description == newDescription
      ? group
      : group with { Description = newDescription };
  }

  public static ExpenseCategoryGroupType Delete(
    this ExpenseCategoryGroupType group)
  {
    return group with { Deleted = true };
  }

  public static ExpenseCategoryGroupType AddExpenseCategory(
    this ExpenseCategoryGroupType group,
    ExpenseCategoryIdType expenseCategoryId)
  {
    return group.ExpenseCategoryIds.Contains(expenseCategoryId)
      ? group
      : group with { ExpenseCategoryIds = group.ExpenseCategoryIds.Append(expenseCategoryId) };
  }

  public static ExpenseCategoryGroupType RemoveExpenseCategory(
    this ExpenseCategoryGroupType group,
    ExpenseCategoryIdType expenseCategoryId)
  {
    return group.ExpenseCategoryIds.Contains(expenseCategoryId)
      ? group
      : group with { ExpenseCategoryIds = group.ExpenseCategoryIds.Without(expenseCategoryId) };
  }
}
