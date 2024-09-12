namespace ExpenseExplorer.Infrastructure.Exceptions;

public sealed class FactSaveException(
  string message,
  Exception innerException)
  : Exception(message, innerException)
{
  public static FactSaveException Wrap(Exception exception)
  {
    return new FactSaveException("Error saving facts", exception);
  }
}
