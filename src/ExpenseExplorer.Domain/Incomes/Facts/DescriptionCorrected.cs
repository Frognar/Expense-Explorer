namespace ExpenseExplorer.Domain.Incomes.Facts;

using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public record DescriptionCorrected(string IncomeId, string Description) : Fact
{
  public static DescriptionCorrected Create(Id incomeId, Description description)
  {
    ArgumentNullException.ThrowIfNull(incomeId);
    ArgumentNullException.ThrowIfNull(description);
    return new DescriptionCorrected(incomeId.Value, description.Value);
  }
}
