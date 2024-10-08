using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Incomes.Facts;

public record SourceCorrected(string IncomeId, string Source) : Fact
{
  public static SourceCorrected Create(Id incomeId, Source source)
  {
    ArgumentNullException.ThrowIfNull(incomeId);
    ArgumentNullException.ThrowIfNull(source);
    return new SourceCorrected(incomeId.Value, source.Name);
  }
}
