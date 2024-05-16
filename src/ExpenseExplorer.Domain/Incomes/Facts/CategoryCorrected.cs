namespace ExpenseExplorer.Domain.Incomes.Facts;

using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public record CategoryCorrected(string IncomeId, string Category) : Fact
{
  public static CategoryCorrected Create(Id incomeId, Category category)
  {
    ArgumentNullException.ThrowIfNull(incomeId);
    ArgumentNullException.ThrowIfNull(category);
    return new CategoryCorrected(incomeId.Value, category.Name);
  }
}
