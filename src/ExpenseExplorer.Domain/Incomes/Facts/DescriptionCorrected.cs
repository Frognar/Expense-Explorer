using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Incomes.Facts;

public record DescriptionCorrected(string IncomeId, string Description) : Fact
{
  public static DescriptionCorrected Create(Id incomeId, Description description)
  {
    ArgumentNullException.ThrowIfNull(incomeId);
    ArgumentNullException.ThrowIfNull(description);
    return new DescriptionCorrected(incomeId.Value, description.Value);
  }
}
