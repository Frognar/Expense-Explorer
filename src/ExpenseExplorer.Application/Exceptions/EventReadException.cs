namespace ExpenseExplorer.Application.Exceptions;

public class EventReadException : Exception
{
  public EventReadException()
  {
  }

  public EventReadException(string message)
    : base(message)
  {
  }

  public EventReadException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  public static EventReadException Wrap(Exception exception)
  {
    return new EventReadException("Error reading events", exception);
  }
}
