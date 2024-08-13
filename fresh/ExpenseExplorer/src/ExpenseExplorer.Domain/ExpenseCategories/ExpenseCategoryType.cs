using DotMaybe;
using DotResult;
using ExpenseExplorer.Domain.ExpenseCategories.Facts;
using ExpenseExplorer.Domain.Facts;
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

  public static Result<ExpenseCategoryType> Recreate(IEnumerable<Fact> facts)
  {
    facts = facts.ToList();
    if (facts.FirstOrDefault() is ExpenseCategoryCreated expenseCategoryCreated)
    {
      return facts.Skip(1)
        .Aggregate(
          Apply(expenseCategoryCreated),
          (expenseCategoryGroup, fact) => expenseCategoryGroup.Bind(r => r.ApplyFact(fact)));
    }

    return Failure.Validation(message: "Invalid expenseCategory facts");
  }

  private static Result<ExpenseCategoryType> ApplyFact(
    this ExpenseCategoryType expenseCategory,
    Fact fact)
  {
    return fact switch
    {
      ExpenseCategoryRenamed expenseCategoryRenamed
        => expenseCategory.Apply(expenseCategoryRenamed),
      ExpenseCategoryDescriptionChanged expenseCategoryDescriptionChanged
        => expenseCategory.Apply(expenseCategoryDescriptionChanged),
      ExpenseCategoryUsageIncreased
        => expenseCategory with { NumberOfUses = expenseCategory.NumberOfUses + 1 },
      ExpenseCategoryUsageDecreased
        => expenseCategory with { NumberOfUses = expenseCategory.NumberOfUses - 1 },
      ExpenseCategoryDeleted => Failure.Validation(message: "Expense category has been deleted"),
      _ => Failure.Validation(message: "Invalid expense category fact"),
    };
  }

  private static Result<ExpenseCategoryType> Apply(ExpenseCategoryCreated fact)
  {
    Maybe<ExpenseCategoryType> expenseCategory =
      from id in ExpenseCategoryId.Create(fact.ExpenseCategoryId)
      from name in Name.Create(fact.Name)
      let description = Description.Create(fact.Description)
      select new ExpenseCategoryType(
        id,
        name,
        description,
        0,
        false,
        UnsavedChanges.Empty(),
        Version.New());

    return expenseCategory.Match(
      () => Failure.Validation(message: "Failed to create expense category"),
      Success.From);
  }

  private static Result<ExpenseCategoryType> Apply(
    this ExpenseCategoryType expenseCategory,
    ExpenseCategoryRenamed fact)
  {
    return (
        from name in Name.Create(fact.Name)
        select expenseCategory with { Name = name })
      .Match(
        () => Failure.Validation(message: "Failed to change name"),
        Success.From);
  }

  private static Result<ExpenseCategoryType> Apply(
    this ExpenseCategoryType expenseCategory,
    ExpenseCategoryDescriptionChanged fact)
  {
    return expenseCategory with { Description = Description.Create(fact.Description) };
  }
}
