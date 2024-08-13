using DotResult;
using ExpenseExplorer.Domain.ExpenseCategories.Facts;
using ExpenseExplorer.Domain.ValueObjects;
using Version = ExpenseExplorer.Domain.ValueObjects.Version;

namespace ExpenseExplorer.Domain.ExpenseCategories;

public readonly record struct ExpenseCategoryType(
  ExpenseCategoryIdType Id,
  NameType Name,
  DescriptionType Description,
  uint NumberOfUses,
  bool Deleted,
  UnsavedChangesType UnsavedChanges,
  VersionType Version);

public static class ExpenseCategory
{
  public static ExpenseCategoryType Create(
    NameType name,
    DescriptionType description)
  {
    ExpenseCategoryIdType expenseCategoryId = ExpenseCategoryId.Unique();
    return new ExpenseCategoryType(
      expenseCategoryId,
      name,
      description,
      0,
      false,
      UnsavedChanges.New(
        ExpenseCategoryCreated.Create(expenseCategoryId, name, description)),
      Version.New());
  }

  public static Result<ExpenseCategoryType> Rename(
    this ExpenseCategoryType category,
    NameType newName)
  {
    if (category.Deleted)
    {
      return Failure.Validation(message: "Cannot rename deleted category");
    }

    return category.Name == newName
      ? category
      : category with { Name = newName, UnsavedChanges = category.UnsavedChanges.Append(ExpenseCategoryRenamed.Create(category.Id, newName)) };
  }

  public static Result<ExpenseCategoryType> ChangeDescription(
    this ExpenseCategoryType category,
    DescriptionType newDescription)
  {
    if (category.Deleted)
    {
      return Failure.Validation(message: "Cannot change description of deleted category");
    }

    return category.Description == newDescription
      ? category
      : category with { Description = newDescription, UnsavedChanges = category.UnsavedChanges.Append(ExpenseCategoryDescriptionChanged.Create(category.Id, newDescription)) };
  }

  public static Result<ExpenseCategoryType> Delete(
    this ExpenseCategoryType category)
  {
    if (category.Deleted)
    {
      return Failure.Validation(message: "Cannot delete already deleted category");
    }

    if (category.NumberOfUses > 0)
    {
      return Failure.Validation(message: "Cannot delete used category");
    }

    return category with { Deleted = true, UnsavedChanges = category.UnsavedChanges.Append(ExpenseCategoryDeleted.Create(category.Id)) };
  }

  public static Result<ExpenseCategoryType> IncreaseUse(
    this ExpenseCategoryType category)
  {
    if (category.Deleted)
    {
      return Failure.Validation(message: "Cannot increase usage of deleted category");
    }

    return category with { NumberOfUses = category.NumberOfUses + 1, UnsavedChanges = category.UnsavedChanges.Append(ExpenseCategoryUsageIncreased.Create(category.Id)) };
  }

  public static Result<ExpenseCategoryType> DecreaseUse(
    this ExpenseCategoryType category)
  {
    if (category.Deleted)
    {
      return Failure.Validation(message: "Cannot decrease usage of deleted category");
    }

    if (category.NumberOfUses <= 0)
    {
      return Failure.Validation(message: "Cannot decrease usage below zero");
    }

    return category with { NumberOfUses = category.NumberOfUses - 1, UnsavedChanges = category.UnsavedChanges.Append(ExpenseCategoryUsageDecreased.Create(category.Id)) };
  }
}
