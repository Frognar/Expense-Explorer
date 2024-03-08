namespace ExpenseExplorer.Domain.Exceptions;

using System.Diagnostics.CodeAnalysis;

public class EmptyStoreNameException : Exception
{
  public EmptyStoreNameException()
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

  public static void ThrowIfEmpty([NotNull] string? name)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new EmptyStoreNameException("Store name cannot be empty.");
    }
  }
}
