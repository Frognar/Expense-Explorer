namespace ExpenseExplorer.Application.Errors;

public record FatalFailure(Exception Exception) : Failure(Exception.ToString())
{
  public Exception Exception { get; } = Exception;
}
