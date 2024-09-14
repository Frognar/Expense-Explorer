using System.Diagnostics;
using DotResult;
using ExpenseExplorer.Domain.ExpenseCategories.Facts;
using ExpenseExplorer.Domain.Extensions;
using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;
using Version = ExpenseExplorer.Domain.ValueObjects.Version;

namespace ExpenseExplorer.Domain.ExpenseCategories;

public sealed record ExpenseCategoryType(
  ExpenseCategoryIdType Id,
  ExpenseCategoryGroupIdType GroupId,
  NameType Name,
  DescriptionType Description,
  uint NumberOfUses,
  bool Deleted,
  UnsavedChangesType UnsavedChanges,
  VersionType Version)
  : EntityType(UnsavedChanges, Version);

public static class ExpenseCategory
{
  public static ExpenseCategoryType Create(
    ExpenseCategoryGroupIdType groupId,
    NameType name,
    DescriptionType description)
  {
    ExpenseCategoryIdType expenseCategoryId = ExpenseCategoryId.Unique();
    Fact fact = ExpenseCategoryCreated.Create(expenseCategoryId, groupId, name, description);
    return new ExpenseCategoryType(
      expenseCategoryId,
      groupId,
      name,
      description,
      0,
      false,
      UnsavedChanges.New(fact),
      Version.New());
  }

  public static Result<ExpenseCategoryType> Rename(
    this ExpenseCategoryType category,
    NameType name)
    => category switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot rename deleted category"),
      not null when category.Name == name => category,
      not null => category with
      {
        Name = name,
        UnsavedChanges = category.UnsavedChanges
          .Append(ExpenseCategoryRenamed.Create(category.Id, name)),
      },
      _ => throw new UnreachableException(),
    };

  public static Result<ExpenseCategoryType> ChangeDescription(
    this ExpenseCategoryType category,
    DescriptionType description)
    => category switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot change description of deleted category"),
      not null when category.Description == description => category,
      not null => category with
      {
        Description = description,
        UnsavedChanges = category.UnsavedChanges
          .Append(ExpenseCategoryDescriptionChanged.Create(category.Id, description)),
      },
      _ => throw new UnreachableException(),
    };

  public static Result<ExpenseCategoryType> Delete(
    this ExpenseCategoryType category)
    => category switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot delete already deleted category"),
      { NumberOfUses: > 0 } => Failure.Validation(message: "Cannot delete used category"),
      not null => category with
      {
        Deleted = true,
        UnsavedChanges = category.UnsavedChanges
          .Append(ExpenseCategoryDeleted.Create(category.Id)),
      },
      _ => throw new UnreachableException(),
    };

  public static Result<ExpenseCategoryType> IncreaseUse(
    this ExpenseCategoryType category)
    => category switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot increase usage of deleted category"),
      not null => category with
      {
        NumberOfUses = category.NumberOfUses + 1,
        UnsavedChanges = category.UnsavedChanges
          .Append(ExpenseCategoryUsageIncreased.Create(category.Id)),
      },
      _ => throw new UnreachableException(),
    };

  public static Result<ExpenseCategoryType> DecreaseUse(
    this ExpenseCategoryType category)
    => category switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot decrease usage of deleted category"),
      { NumberOfUses: <= 0 } => Failure.Validation(message: "Cannot decrease usage below zero"),
      not null => category with
      {
        NumberOfUses = category.NumberOfUses - 1,
        UnsavedChanges = category.UnsavedChanges
          .Append(ExpenseCategoryUsageDecreased.Create(category.Id)),
      },
      _ => throw new UnreachableException(),
    };

  public static Result<ExpenseCategoryType> ClearChanges(
    this ExpenseCategoryType category)
    => category switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot clear changes of deleted category"),
      not null => category with { UnsavedChanges = UnsavedChanges.Empty(), },
      _ => throw new UnreachableException(),
    };

  public static Result<ExpenseCategoryType> Recreate(IEnumerable<Fact> facts)
    => facts.ToList() switch
    {
      [ExpenseCategoryCreated created] => Apply(created),
      [ExpenseCategoryCreated created, .. var rest] => rest.Aggregate(Apply(created), ApplyFact),
      _ => Failure.Validation(message: "Invalid expenseCategory facts"),
    };

  private static Result<ExpenseCategoryType> ApplyFact(
    this Result<ExpenseCategoryType> category,
    Fact fact)
    => category.Bind(c => c.ApplyFact(fact));

  private static Result<ExpenseCategoryType> ApplyFact(
    this ExpenseCategoryType category,
    Fact fact)
    => fact switch
    {
      ExpenseCategoryRenamed renamed => category.Apply(renamed),
      ExpenseCategoryDescriptionChanged descriptionChanged => category.Apply(descriptionChanged),
      ExpenseCategoryUsageIncreased => category with { NumberOfUses = category.NumberOfUses + 1 },
      ExpenseCategoryUsageDecreased => category with { NumberOfUses = category.NumberOfUses - 1 },
      ExpenseCategoryDeleted => Failure.Validation(message: "Expense category has been deleted"),
      _ => Failure.Validation(message: "Invalid expense category fact"),
    };

  private static Result<ExpenseCategoryType> Apply(ExpenseCategoryCreated fact)
    => (
        from id in ExpenseCategoryId.Create(fact.ExpenseCategoryId)
        from groupId in ExpenseCategoryGroupId.Create(fact.ExpenseCategoryGroupId)
        from name in Name.Create(fact.Name)
        let description = Description.Create(fact.Description)
        select new ExpenseCategoryType(
          id,
          groupId,
          name,
          description,
          0,
          false,
          UnsavedChanges.Empty(),
          Version.New()))
      .ToResult(() => Failure.Validation(message: "Failed to create expense category"));

  private static Result<ExpenseCategoryType> Apply(
    this ExpenseCategoryType expenseCategory,
    ExpenseCategoryRenamed fact)
    => (
        from name in Name.Create(fact.Name)
        select expenseCategory with { Name = name })
      .ToResult(() => Failure.Validation(message: "Failed to change name"));

  private static Result<ExpenseCategoryType> Apply(
    this ExpenseCategoryType expenseCategory,
    ExpenseCategoryDescriptionChanged fact)
    => expenseCategory with { Description = Description.Create(fact.Description) };
}
