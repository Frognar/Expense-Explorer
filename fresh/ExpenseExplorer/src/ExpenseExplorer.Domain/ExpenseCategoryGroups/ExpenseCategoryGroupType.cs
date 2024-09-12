using DotResult;
using ExpenseExplorer.Domain.ExpenseCategoryGroups.Facts;
using ExpenseExplorer.Domain.Extensions;
using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;
using Version = ExpenseExplorer.Domain.ValueObjects.Version;

namespace ExpenseExplorer.Domain.ExpenseCategoryGroups;

public sealed record ExpenseCategoryGroupType(
  ExpenseCategoryGroupIdType Id,
  NameType Name,
  DescriptionType Description,
  ExpenseCategoryIdsType ExpenseCategoryIds,
  bool Deleted,
  UnsavedChangesType UnsavedChanges,
  VersionType Version)
  : EntityType(UnsavedChanges, Version);

#pragma warning disable SA1402
public static class ExpenseCategoryGroup
{
  public static ExpenseCategoryGroupType Create(
    NameType name,
    DescriptionType description)
  {
    ExpenseCategoryGroupIdType expenseCategoryGroupId = ExpenseCategoryGroupId.Unique();
    Fact fact = ExpenseCategoryGroupCreated.Create(expenseCategoryGroupId, name, description);
    return new ExpenseCategoryGroupType(
      expenseCategoryGroupId,
      name,
      description,
      ExpenseCategoryIds.New(),
      false,
      UnsavedChanges.New(fact),
      Version.New());
  }

  public static Result<ExpenseCategoryGroupType> Rename(
    this ExpenseCategoryGroupType categoryGroup,
    NameType name)
    => categoryGroup switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot rename deleted group"),
      { } when categoryGroup.Name == name => categoryGroup,
      { } => categoryGroup with
      {
        Name = name,
        UnsavedChanges = categoryGroup.UnsavedChanges
          .Append(ExpenseCategoryGroupRenamed.Create(categoryGroup.Id, name)),
      },
      _ => Failure.Fatal(message: "Category group is null"),
    };

  public static Result<ExpenseCategoryGroupType> ChangeDescription(
    this ExpenseCategoryGroupType categoryGroup,
    DescriptionType description)
    => categoryGroup switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot change description of deleted group"),
      { } when categoryGroup.Description == description => categoryGroup,
      { } => categoryGroup with
      {
        Description = description,
        UnsavedChanges = categoryGroup.UnsavedChanges
          .Append(ExpenseCategoryGroupDescriptionChanged.Create(categoryGroup.Id, description)),
      },
      _ => Failure.Fatal(message: "Category group is null"),
    };

  public static Result<ExpenseCategoryGroupType> Delete(
    this ExpenseCategoryGroupType categoryGroup)
    => categoryGroup switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot delete already deleted group"),
      { } => categoryGroup with
      {
        Deleted = true,
        UnsavedChanges = categoryGroup.UnsavedChanges
          .Append(ExpenseCategoryGroupDeleted.Create(categoryGroup.Id)),
      },
      _ => Failure.Fatal(message: "Category group is null"),
    };

  public static Result<ExpenseCategoryGroupType> AddExpenseCategory(
    this ExpenseCategoryGroupType categoryGroup,
    ExpenseCategoryIdType categoryId)
    => categoryGroup switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot add expense category to deleted group"),
      { } when categoryGroup.ExpenseCategoryIds.Contains(categoryId) => categoryGroup,
      { } => categoryGroup with
      {
        ExpenseCategoryIds = categoryGroup.ExpenseCategoryIds.Append(categoryId),
        UnsavedChanges = categoryGroup.UnsavedChanges
          .Append(ExpenseCategoryGroupExpenseCategoryAdded.Create(categoryGroup.Id, categoryId)),
      },
      _ => Failure.Fatal(message: "Category group is null"),
    };

  public static Result<ExpenseCategoryGroupType> RemoveExpenseCategory(
    this ExpenseCategoryGroupType categoryGroup,
    ExpenseCategoryIdType categoryId)
    => categoryGroup switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot remove expense category from deleted group"),
      { } when !categoryGroup.ExpenseCategoryIds.Contains(categoryId) => categoryGroup,
      { } => categoryGroup with
      {
        ExpenseCategoryIds = categoryGroup.ExpenseCategoryIds.Without(categoryId),
        UnsavedChanges = categoryGroup.UnsavedChanges
          .Append(ExpenseCategoryGroupExpenseCategoryRemoved.Create(categoryGroup.Id, categoryId)),
      },
      _ => Failure.Fatal(message: "Category group is null"),
    };

  public static Result<ExpenseCategoryGroupType> ClearChanges(
    this ExpenseCategoryGroupType categoryGroup)
    => categoryGroup switch
    {
      { Deleted: true } => Failure.Validation(message: "Cannot clear changes of deleted group"),
      { } => categoryGroup with { UnsavedChanges = UnsavedChanges.Empty() },
      _ => Failure.Fatal(message: "Category group is null"),
    };

  public static Result<ExpenseCategoryGroupType> Recreate(
    IEnumerable<Fact> facts)
    => facts.ToList() switch
    {
      [ExpenseCategoryGroupCreated created] => Apply(created),
      [ExpenseCategoryGroupCreated created, .. { } rest] => rest.Aggregate(Apply(created), ApplyFact),
      _ => Failure.Validation(message: "Invalid expenseCategoryGroup facts"),
    };

  private static Result<ExpenseCategoryGroupType> ApplyFact(
    Result<ExpenseCategoryGroupType> categoryGroup,
    Fact fact)
    => categoryGroup.Bind(c => c.ApplyFact(fact));

  private static Result<ExpenseCategoryGroupType> ApplyFact(
    this ExpenseCategoryGroupType categoryGroup,
    Fact fact)
    => fact switch
    {
      ExpenseCategoryGroupRenamed renamed => categoryGroup.Apply(renamed),
      ExpenseCategoryGroupDescriptionChanged descriptionChanged => categoryGroup.Apply(descriptionChanged),
      ExpenseCategoryGroupExpenseCategoryAdded categoryAdded => categoryGroup.Apply(categoryAdded),
      ExpenseCategoryGroupExpenseCategoryRemoved categoryRemoved => categoryGroup.Apply(categoryRemoved),
      ExpenseCategoryGroupDeleted => Failure.Validation(message: "Expense category group has been deleted"),
      _ => Failure.Validation(message: "Invalid expense category group fact"),
    };

  private static Result<ExpenseCategoryGroupType> Apply(
    ExpenseCategoryGroupCreated fact)
    => (
        from id in ExpenseCategoryGroupId.Create(fact.ExpenseCategoryGroupId)
        from name in Name.Create(fact.Name)
        let description = Description.Create(fact.Description)
        select new ExpenseCategoryGroupType(
          id,
          name,
          description,
          ExpenseCategoryIds.New(),
          false,
          UnsavedChanges.Empty(),
          Version.New()))
      .ToResult(() => Failure.Validation(message: "Failed to create expense category group"));

  private static Result<ExpenseCategoryGroupType> Apply(
    this ExpenseCategoryGroupType categoryGroup,
    ExpenseCategoryGroupRenamed fact)
    => (
        from name in Name.Create(fact.Name)
        select categoryGroup with { Name = name })
      .ToResult(() => Failure.Validation(message: "Failed to change name"));

  private static Result<ExpenseCategoryGroupType> Apply(
    this ExpenseCategoryGroupType categoryGroup,
    ExpenseCategoryGroupDescriptionChanged fact)
    => categoryGroup with { Description = Description.Create(fact.Description) };

  private static Result<ExpenseCategoryGroupType> Apply(
    this ExpenseCategoryGroupType categoryGroup,
    ExpenseCategoryGroupExpenseCategoryAdded fact)
    => (
        from expenseCategoryId in ExpenseCategoryId.Create(fact.ExpenseCategoryId)
        let expenseCategoryIds = categoryGroup.ExpenseCategoryIds.Append(expenseCategoryId)
        select categoryGroup with { ExpenseCategoryIds = expenseCategoryIds })
      .ToResult(() => Failure.Validation(message: "Failed to add expense category"));

  private static Result<ExpenseCategoryGroupType> Apply(
    this ExpenseCategoryGroupType categoryGroup,
    ExpenseCategoryGroupExpenseCategoryRemoved fact)
    => (
        from expenseCategoryId in ExpenseCategoryId.Create(fact.ExpenseCategoryId)
        let expenseCategoryIds = categoryGroup.ExpenseCategoryIds.Without(expenseCategoryId)
        select categoryGroup with { ExpenseCategoryIds = expenseCategoryIds })
      .ToResult(() => Failure.Validation(message: "Failed to remove expense category"));
}
