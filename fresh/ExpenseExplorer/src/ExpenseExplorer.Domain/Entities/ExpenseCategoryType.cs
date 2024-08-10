using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Entities;

public readonly record struct ExpenseCategoryType(
  ExpenseCategoryIdType Id,
  NameType Name,
  DescriptionType Description,
  uint NumberOfUses,
  bool Deleted);

public static class ExpenseCategory
{
  public static ExpenseCategoryType Create(
    NameType name,
    DescriptionType description)
  {
    return new ExpenseCategoryType(ExpenseCategoryId.Unique(), name, description, 0, false);
  }

  public static ExpenseCategoryType Rename(
    this ExpenseCategoryType category,
    NameType newName)
  {
    return category.Name == newName
      ? category
      : category with { Name = newName };
  }

  public static ExpenseCategoryType ChangeDescription(
    this ExpenseCategoryType category,
    DescriptionType newDescription)
  {
    return category.Description == newDescription
      ? category
      : category with { Description = newDescription };
  }

  public static ExpenseCategoryType Delete(
    this ExpenseCategoryType category)
  {
    return category with { Deleted = true };
  }

  public static ExpenseCategoryType IncreaseUse(
    this ExpenseCategoryType category)
  {
    return category with { NumberOfUses = category.NumberOfUses + 1 };
  }

  public static ExpenseCategoryType DecreaseUse(
    this ExpenseCategoryType category)
  {
    return category with { NumberOfUses = category.NumberOfUses - 1 };
  }
}
