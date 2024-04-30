namespace ExpenseExplorer.Domain.Exceptions;

public class EmptyStoreNameException : Exception
{
  public EmptyStoreNameException()
    : base("Store name cannot be empty.")
  {
  }

  public EmptyStoreNameException(string message)
    : base(message)
  {
  }

  public EmptyStoreNameException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}
