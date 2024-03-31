namespace ExpenseExplorer.Application.Exceptions;

public class FactReadException : Exception
{
  public FactReadException()
  {
  }

  public FactReadException(string message)
    : base(message)
  {
  }

  public FactReadException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  public static FactReadException Wrap(Exception exception)
  {
    return new FactReadException("Error reading facts", exception);
  }
}
