namespace ExpenseExplorer.Domain.Exceptions;

public class EmptyCategoryNameException : Exception
{
  public EmptyCategoryNameException()
    : base("Category name cannot be empty.")
  {
  }

  public EmptyCategoryNameException(string message)
    : base(message)
  {
  }

  public EmptyCategoryNameException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}
