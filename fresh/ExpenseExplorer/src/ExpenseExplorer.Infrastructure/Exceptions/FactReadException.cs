namespace ExpenseExplorer.Infrastructure.Exceptions;

public sealed class FactReadException(
  string message,
  Exception innerException)
  : Exception(message, innerException)
{
  public static FactReadException Wrap(Exception exception)
  {
    return new FactReadException("Error reading facts", exception);
  }
}
