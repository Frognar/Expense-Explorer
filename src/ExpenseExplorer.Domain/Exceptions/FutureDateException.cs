namespace ExpenseExplorer.Domain.Exceptions;

public class FutureDateException : Exception
{
  public FutureDateException()
    : base("Purchase date cannot be in the future")
  {
  }

  public FutureDateException(string message)
    : base(message)
  {
  }

  public FutureDateException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  public FutureDateException(DateOnly date, DateOnly today)
    : base($"Purchase date cannot be in the future. Given date: {date:yyyy-MM-dd}, today: {today:yyyy-MM-dd}")
  {
  }
}
