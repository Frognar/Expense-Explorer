namespace ExpenseExplorer.Domain.Incomes.Facts;

using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public record ReceivedDateCorrected(string IncomeId, DateOnly ReceivedDate, DateOnly RequestedDate) : Fact
{
  public static ReceivedDateCorrected Create(Id incomeId, NonFutureDate receivedDate, DateOnly requestedDate)
  {
    ArgumentNullException.ThrowIfNull(incomeId);
    ArgumentNullException.ThrowIfNull(receivedDate);
    return new ReceivedDateCorrected(incomeId.Value, receivedDate.Date, requestedDate);
  }
}
