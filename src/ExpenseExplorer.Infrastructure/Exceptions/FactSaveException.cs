namespace ExpenseExplorer.Infrastructure.Exceptions;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Not needed")]
public class FactSaveException(string message, Exception innerException) : Exception(message, innerException)
{
  public static FactSaveException Wrap(Exception exception)
  {
    return new FactSaveException("Error saving facts", exception);
  }
}
