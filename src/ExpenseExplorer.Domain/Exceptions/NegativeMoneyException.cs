namespace ExpenseExplorer.Domain.Exceptions;

public class NegativeMoneyException : Exception
{
  public NegativeMoneyException()
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

  public static void ThrowIfNegative(decimal value)
  {
    if (value < 0)
    {
      throw new NegativeMoneyException("Money cannot be negative.");
    }
  }
}
