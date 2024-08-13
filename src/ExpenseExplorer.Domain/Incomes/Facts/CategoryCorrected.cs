using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Incomes.Facts;

public record CategoryCorrected(string IncomeId, string Category) : Fact
{
  public static CategoryCorrected Create(Id incomeId, Category category)
  {
    ArgumentNullException.ThrowIfNull(incomeId);
    ArgumentNullException.ThrowIfNull(category);
    return new CategoryCorrected(incomeId.Value, category.Name);
  }
}
