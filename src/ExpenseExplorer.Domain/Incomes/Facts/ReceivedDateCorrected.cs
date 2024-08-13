using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Incomes.Facts;

public record ReceivedDateCorrected(string IncomeId, DateOnly ReceivedDate, DateOnly RequestedDate) : Fact
{
  public static ReceivedDateCorrected Create(Id incomeId, NonFutureDate receivedDate, DateOnly requestedDate)
  {
    ArgumentNullException.ThrowIfNull(incomeId);
    ArgumentNullException.ThrowIfNull(receivedDate);
    return new ReceivedDateCorrected(incomeId.Value, receivedDate.Date, requestedDate);
  }
}
