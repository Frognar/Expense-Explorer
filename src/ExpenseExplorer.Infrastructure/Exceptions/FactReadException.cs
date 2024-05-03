namespace ExpenseExplorer.Infrastructure.Exceptions;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Not needed")]
public class FactReadException(string message, Exception innerException) : Exception(message, innerException)
{
  public static FactReadException Wrap(Exception exception)
  {
    return new FactReadException("Error reading facts", exception);
  }
}
