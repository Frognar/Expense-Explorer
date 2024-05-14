namespace ExpenseExplorer.Domain.Incomes.Facts;

using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public sealed record IncomeCreated(
  string IncomeId,
  string Source,
  decimal Amount,
  DateOnly ReceivedDate,
  string Category,
  string Description,
  DateOnly CreatedDate) : Fact
{
  public static IncomeCreated Create(
    Id id,
    Source source,
    Money amount,
    NonFutureDate receivedDate,
    Category category,
    Description description,
    DateOnly createdDate)
  {
    ArgumentNullException.ThrowIfNull(id);
    ArgumentNullException.ThrowIfNull(source);
    ArgumentNullException.ThrowIfNull(amount);
    ArgumentNullException.ThrowIfNull(receivedDate);
    ArgumentNullException.ThrowIfNull(category);
    ArgumentNullException.ThrowIfNull(description);
    ArgumentNullException.ThrowIfNull(createdDate);
    return new IncomeCreated(id.Value, source.Name, amount.Value, receivedDate.Date, category.Name, description.Value, createdDate);
  }
}
