namespace ExpenseExplorer.Domain.Exceptions;

using System.Diagnostics.CodeAnalysis;

public class EmptyItemNameException : Exception
{
  public EmptyItemNameException()
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

  public static void ThrowIfEmpty([NotNull] string? name)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new EmptyItemNameException("Item name cannot be empty.");
    }
  }
}
