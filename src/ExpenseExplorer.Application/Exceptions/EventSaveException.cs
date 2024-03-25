namespace ExpenseExplorer.Application.Exceptions;

public class EventSaveException : Exception
{
  public EventSaveException()
  {
  }

  public EventSaveException(string message)
    : base(message)
  {
  }

  public EventSaveException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  public static EventSaveException Wrap(Exception exception)
  {
    return new EventSaveException("Error saving events", exception);
  }
}
