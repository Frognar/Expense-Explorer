namespace ExpenseExplorer.Domain.Incomes.Facts;

using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public record IncomeDeleted(string IncomeId) : Fact
{
  public static IncomeDeleted Create(Id incomeId)
  {
    ArgumentNullException.ThrowIfNull(incomeId);
    return new IncomeDeleted(incomeId.Value);
  }
}
