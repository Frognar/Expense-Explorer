namespace ExpenseExplorer.Domain.Exceptions;

public class NonPositiveQuantityException : Exception
{
  public NonPositiveQuantityException()
  {
  }

  public NonPositiveQuantityException(string message)
    : base(message)
  {
  }

  public NonPositiveQuantityException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  public static void ThrowIfNotPositive(decimal value)
  {
    if (value <= 0)
    {
      throw new NonPositiveQuantityException("Quantity must be positive");
    }
  }
}
