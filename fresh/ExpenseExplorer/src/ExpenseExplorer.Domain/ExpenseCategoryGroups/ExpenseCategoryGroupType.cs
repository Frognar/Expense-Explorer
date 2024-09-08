using DotMaybe;
using DotResult;
using ExpenseExplorer.Domain.ExpenseCategoryGroups.Facts;
using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;
using Version = ExpenseExplorer.Domain.ValueObjects.Version;

namespace ExpenseExplorer.Domain.ExpenseCategoryGroups;

public readonly record struct ExpenseCategoryGroupType(
  ExpenseCategoryGroupIdType Id,
  NameType Name,
  DescriptionType Description,
  ExpenseCategoryIdsType ExpenseCategoryIds,
  bool Deleted,
  UnsavedChangesType UnsavedChanges,
  VersionType Version);

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
    this ExpenseCategoryGroupType group,
    NameType newName)
  {
    if (group.Deleted)
    {
      return Failure.Validation(message: "Cannot rename deleted group");
    }

    if (group.Name == newName)
    {
      return group;
    }

    Fact fact = ExpenseCategoryGroupRenamed.Create(group.Id, newName);
    return group with { Name = newName, UnsavedChanges = group.UnsavedChanges.Append(fact) };
  }

  public static Result<ExpenseCategoryGroupType> ChangeDescription(
    this ExpenseCategoryGroupType group,
    DescriptionType newDescription)
  {
    if (group.Deleted)
    {
      return Failure.Validation(message: "Cannot change description of deleted group");
    }

    if (group.Description == newDescription)
    {
      return group;
    }

    Fact fact = ExpenseCategoryGroupDescriptionChanged.Create(group.Id, newDescription);
    return group with { Description = newDescription, UnsavedChanges = group.UnsavedChanges.Append(fact) };
  }

  public static Result<ExpenseCategoryGroupType> Delete(
    this ExpenseCategoryGroupType group)
  {
    if (group.Deleted)
    {
      return Failure.Validation(message: "Cannot delete already deleted group");
    }

    Fact fact = ExpenseCategoryGroupDeleted.Create(group.Id);
    return group with { Deleted = true, UnsavedChanges = group.UnsavedChanges.Append(fact) };
  }

  public static Result<ExpenseCategoryGroupType> AddExpenseCategory(
    this ExpenseCategoryGroupType group,
    ExpenseCategoryIdType expenseCategoryId)
  {
    if (group.Deleted)
    {
      return Failure.Validation(message: "Cannot add expense category to deleted group");
    }

    if (group.ExpenseCategoryIds.Contains(expenseCategoryId))
    {
      return group;
    }

    ExpenseCategoryIdsType expenseCategoryIds = group.ExpenseCategoryIds.Append(expenseCategoryId);
    Fact fact = ExpenseCategoryGroupExpenseCategoryAdded.Create(group.Id, expenseCategoryId);
    return group with { ExpenseCategoryIds = expenseCategoryIds, UnsavedChanges = group.UnsavedChanges.Append(fact) };
  }

  public static Result<ExpenseCategoryGroupType> RemoveExpenseCategory(
    this ExpenseCategoryGroupType group,
    ExpenseCategoryIdType expenseCategoryId)
  {
    if (group.Deleted)
    {
      return Failure.Validation(message: "Cannot remove expense category from deleted group");
    }

    if (!group.ExpenseCategoryIds.Contains(expenseCategoryId))
    {
      return group;
    }

    ExpenseCategoryIdsType expenseCategoryIds = group.ExpenseCategoryIds.Without(expenseCategoryId);
    Fact fact = ExpenseCategoryGroupExpenseCategoryRemoved.Create(group.Id, expenseCategoryId);
    return group with { ExpenseCategoryIds = expenseCategoryIds, UnsavedChanges = group.UnsavedChanges.Append(fact) };
  }

  public static Result<ExpenseCategoryGroupType> ClearChanges(
    this ExpenseCategoryGroupType group)
  {
    if (group.Deleted)
    {
      return Failure.Validation(message: "Cannot clear changes of deleted group");
    }

    return group with { UnsavedChanges = UnsavedChanges.Empty() };
  }

  public static Result<ExpenseCategoryGroupType> Recreate(IEnumerable<Fact> facts)
  {
    return facts.ToList() switch
    {
      [ExpenseCategoryGroupCreated created] => Apply(created),
      [ExpenseCategoryGroupCreated created, .. { } rest] => rest.Aggregate(Apply(created), ApplyFact),
      _ => Failure.Validation(message: "Invalid expenseCategoryGroup facts"),
    };
  }

  private static Result<ExpenseCategoryGroupType> ApplyFact(Result<ExpenseCategoryGroupType> categoryGroup, Fact fact)
  {
    return categoryGroup.Bind(c => c.ApplyFact(fact));
  }

  private static Result<ExpenseCategoryGroupType> ApplyFact(this ExpenseCategoryGroupType categoryGroup, Fact fact)
  {
    return fact switch
    {
      ExpenseCategoryGroupRenamed renamed => categoryGroup.Apply(renamed),
      ExpenseCategoryGroupDescriptionChanged descriptionChanged => categoryGroup.Apply(descriptionChanged),
      ExpenseCategoryGroupExpenseCategoryAdded categoryAdded => categoryGroup.Apply(categoryAdded),
      ExpenseCategoryGroupExpenseCategoryRemoved categoryRemoved => categoryGroup.Apply(categoryRemoved),
      ExpenseCategoryGroupDeleted => Failure.Validation(message: "Expense category group has been deleted"),
      _ => Failure.Validation(message: "Invalid expense category group fact"),
    };
  }

  private static Result<ExpenseCategoryGroupType> Apply(ExpenseCategoryGroupCreated fact)
  {
    Maybe<ExpenseCategoryGroupType> expenseCategoryGroup =
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
        Version.New());

    return expenseCategoryGroup.Match(
      () => Failure.Validation(message: "Failed to create expense category group"),
      Success.From);
  }

  private static Result<ExpenseCategoryGroupType> Apply(
    this ExpenseCategoryGroupType expenseCategoryGroup,
    ExpenseCategoryGroupRenamed fact)
  {
    return (
        from name in Name.Create(fact.Name)
        select expenseCategoryGroup with { Name = name })
      .Match(
        () => Failure.Validation(message: "Failed to change name"),
        Success.From);
  }

  private static Result<ExpenseCategoryGroupType> Apply(
    this ExpenseCategoryGroupType expenseCategoryGroup,
    ExpenseCategoryGroupDescriptionChanged fact)
  {
    return expenseCategoryGroup with { Description = Description.Create(fact.Description) };
  }

  private static Result<ExpenseCategoryGroupType> Apply(
    this ExpenseCategoryGroupType expenseCategoryGroup,
    ExpenseCategoryGroupExpenseCategoryAdded fact)
  {
    return (
        from expenseCategoryId in ExpenseCategoryId.Create(fact.ExpenseCategoryId)
        let expenseCategoryIds = expenseCategoryGroup.ExpenseCategoryIds.Append(expenseCategoryId)
        select expenseCategoryGroup with { ExpenseCategoryIds = expenseCategoryIds })
      .Match(
        () => Failure.Validation(message: "Failed to add expense category"),
        Success.From);
  }

  private static Result<ExpenseCategoryGroupType> Apply(
    this ExpenseCategoryGroupType expenseCategoryGroup,
    ExpenseCategoryGroupExpenseCategoryRemoved fact)
  {
    return (
        from expenseCategoryId in ExpenseCategoryId.Create(fact.ExpenseCategoryId)
        let expenseCategoryIds = expenseCategoryGroup.ExpenseCategoryIds.Without(expenseCategoryId)
        select expenseCategoryGroup with { ExpenseCategoryIds = expenseCategoryIds })
      .Match(
        () => Failure.Validation(message: "Failed to remove expense category"),
        Success.From);
  }
}
