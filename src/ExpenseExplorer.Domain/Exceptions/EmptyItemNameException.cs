namespace ExpenseExplorer.Domain.Exceptions;

public class EmptyItemNameException : Exception
{
  public EmptyItemNameException()
    : base("Item name cannot be empty.")
  {
  }

  public EmptyItemNameException(string message)
    : base(message)
  {
  }

  public EmptyItemNameException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}
