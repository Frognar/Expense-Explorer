namespace ExpenseExplorer.Application.Exceptions;

public class FactSaveException : Exception
{
  public FactSaveException()
  {
  }

  public FactSaveException(string message)
    : base(message)
  {
  }

  public FactSaveException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  public static FactSaveException Wrap(Exception exception)
  {
    return new FactSaveException("Error saving facts", exception);
  }
}
