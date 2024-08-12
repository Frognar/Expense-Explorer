using System.Diagnostics.CodeAnalysis;

namespace ExpenseExplorer.Infrastructure.Exceptions;

[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Not needed")]
public sealed class FactSaveException(
  string message,
  Exception innerException)
  : Exception(message, innerException)
{
  public static FactSaveException Wrap(Exception exception)
  {
    return new FactSaveException("Error reading facts", exception);
  }
}
