namespace ExpenseExplorer.Domain.Exceptions;

using System.Diagnostics.CodeAnalysis;

public class EmptyCategoryNameException : Exception
{
  public EmptyCategoryNameException()
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

  public static void ThrowIfEmpty([NotNull] string? name)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new EmptyCategoryNameException("Category name cannot be empty.");
    }
  }
}
