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
          description)),
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

  public static Result<ExpenseCategoryGroupType> Recreate(IEnumerable<Fact> facts)
  {
    facts = facts.ToList();
    if (facts.FirstOrDefault() is ExpenseCategoryGroupCreated expenseCategoryGroupCreated)
    {
      return facts.Skip(1)
        .Aggregate(
          Apply(expenseCategoryGroupCreated),
          (expenseCategoryGroup, fact) => expenseCategoryGroup.Bind(r => r.ApplyFact(fact)));
    }

    return Failure.Validation(message: "Invalid expenseCategoryGroup facts");
  }

  private static Result<ExpenseCategoryGroupType> ApplyFact(
    this ExpenseCategoryGroupType expenseCategoryGroup,
    Fact fact)
  {
    return fact switch
    {
      ExpenseCategoryGroupRenamed expenseCategoryGroupRenamed
        => expenseCategoryGroup.Apply(expenseCategoryGroupRenamed),
      ExpenseCategoryGroupDescriptionChanged expenseCategoryGroupDescriptionChanged
        => expenseCategoryGroup.Apply(expenseCategoryGroupDescriptionChanged),
      ExpenseCategoryGroupExpenseCategoryAdded expenseCategoryGroupExpenseCategoryAdded
        => expenseCategoryGroup.Apply(expenseCategoryGroupExpenseCategoryAdded),
      ExpenseCategoryGroupExpenseCategoryRemoved expenseCategoryGroupExpenseCategoryRemoved
        => expenseCategoryGroup.Apply(expenseCategoryGroupExpenseCategoryRemoved),
      ExpenseCategoryGroupDeleted => Failure.Validation(message: "Expense category group has been deleted"),
      _ => Failure.Validation(message: "Invalid expense category group fact"),
    };
  }

  private static Result<ExpenseCategoryGroupType> Apply(
    ExpenseCategoryGroupCreated fact)
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
        select expenseCategoryGroup with { ExpenseCategoryIds = expenseCategoryGroup.ExpenseCategoryIds.Append(expenseCategoryId) })
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
        select expenseCategoryGroup with { ExpenseCategoryIds = expenseCategoryGroup.ExpenseCategoryIds.Without(expenseCategoryId) })
      .Match(
        () => Failure.Validation(message: "Failed to remove expense category"),
        Success.From);
  }
}
