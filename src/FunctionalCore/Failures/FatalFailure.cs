namespace FunctionalCore.Failures;

public record FatalFailure(Exception Exception) : Failure(Exception.ToString())
{
  public Exception Exception { get; } = Exception;
}
