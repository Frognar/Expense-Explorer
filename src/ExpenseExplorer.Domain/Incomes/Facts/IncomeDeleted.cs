using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Incomes.Facts;

public record IncomeDeleted(string IncomeId) : Fact
{
  public static IncomeDeleted Create(Id incomeId)
  {
    ArgumentNullException.ThrowIfNull(incomeId);
    return new IncomeDeleted(incomeId.Value);
  }
}
