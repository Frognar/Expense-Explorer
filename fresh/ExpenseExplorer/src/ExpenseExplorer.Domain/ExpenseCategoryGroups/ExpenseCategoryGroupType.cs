using DotResult;
using ExpenseExplorer.Domain.ExpenseCategoryGroups.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.ExpenseCategoryGroups;

public readonly record struct ExpenseCategoryGroupType(
  ExpenseCategoryGroupIdType Id,
  NameType Name,
  DescriptionType Description,
  ExpenseCategoryIdsType ExpenseCategoryIds,
  bool Deleted,
  UnsavedChangesType UnsavedChanges);

public static class ExpenseCategoryGroup
{
  public static ExpenseCategoryGroupType Create(
    NameType name,
    DescriptionType description,
    ExpenseCategoryIdsType expenseCategoryIds)
  {
    ExpenseCategoryGroupIdType expenseCategoryGroupId = ExpenseCategoryGroupId.Unique();
    return new ExpenseCategoryGroupType(
      expenseCategoryGroupId,
      name,
      description,
      expenseCategoryIds,
      false,
      UnsavedChanges.New(
        ExpenseCategoryGroupCreated.Create(
          expenseCategoryGroupId,
          name,
          description)));
  }

  public static Result<ExpenseCategoryGroupType> Rename(
    this ExpenseCategoryGroupType group,
    NameType newName)
  {
    if (group.Deleted)
    {
      return Failure.Validation(message: "Cannot rename deleted group");
    }

    return group.Name == newName
      ? group
      : group with { Name = newName, UnsavedChanges = group.UnsavedChanges.Append(ExpenseCategoryGroupRenamed.Create(group.Id, newName)) };
  }

  public static Result<ExpenseCategoryGroupType> ChangeDescription(
    this ExpenseCategoryGroupType group,
    DescriptionType newDescription)
  {
    if (group.Deleted)
    {
      return Failure.Validation(message: "Cannot change description of deleted group");
    }

    return group.Description == newDescription
      ? group
      : group with { Description = newDescription, UnsavedChanges = group.UnsavedChanges.Append(ExpenseCategoryGroupDescriptionChanged.Create(group.Id, newDescription)) };
  }

  public static Result<ExpenseCategoryGroupType> Delete(
    this ExpenseCategoryGroupType group)
  {
    if (group.Deleted)
    {
      return Failure.Validation(message: "Cannot delete already deleted group");
    }

    return group with { Deleted = true, UnsavedChanges = group.UnsavedChanges.Append(ExpenseCategoryGroupDeleted.Create(group.Id)) };
  }

  public static Result<ExpenseCategoryGroupType> AddExpenseCategory(
    this ExpenseCategoryGroupType group,
    ExpenseCategoryIdType expenseCategoryId)
  {
    if (group.Deleted)
    {
      return Failure.Validation(message: "Cannot add expense category to deleted group");
    }

    return group.ExpenseCategoryIds.Contains(expenseCategoryId)
      ? group
      : group with { ExpenseCategoryIds = group.ExpenseCategoryIds.Append(expenseCategoryId), UnsavedChanges = group.UnsavedChanges.Append(ExpenseCategoryGroupExpenseCategoryAdded.Create(group.Id, expenseCategoryId)) };
  }

  public static Result<ExpenseCategoryGroupType> RemoveExpenseCategory(
    this ExpenseCategoryGroupType group,
    ExpenseCategoryIdType expenseCategoryId)
  {
    if (group.Deleted)
    {
      return Failure.Validation(message: "Cannot remove expense category from deleted group");
    }

    return group.ExpenseCategoryIds.Contains(expenseCategoryId)
      ? group with { ExpenseCategoryIds = group.ExpenseCategoryIds.Without(expenseCategoryId), UnsavedChanges = group.UnsavedChanges.Append(ExpenseCategoryGroupExpenseCategoryRemoved.Create(group.Id, expenseCategoryId)) }
      : group;
  }
}
