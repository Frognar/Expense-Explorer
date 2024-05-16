namespace ExpenseExplorer.Domain.Incomes.Facts;

using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public record AmountCorrected(string IncomeId, decimal Amount) : Fact
{
  public static AmountCorrected Create(Id incomeId, Money amount)
  {
    ArgumentNullException.ThrowIfNull(incomeId);
    ArgumentNullException.ThrowIfNull(amount);
    return new AmountCorrected(incomeId.Value, amount.Value);
  }
}
