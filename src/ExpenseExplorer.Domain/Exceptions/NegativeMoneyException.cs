namespace ExpenseExplorer.Domain.Exceptions;

public class NegativeMoneyException : Exception
{
  public NegativeMoneyException()
    : base("Money cannot be negative.")
  {
  }

  public NegativeMoneyException(string message)
    : base(message)
  {
  }

  public NegativeMoneyException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}
