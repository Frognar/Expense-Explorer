namespace FunctionalCore.Failures;

public record FatalFailure(Exception Exception) : Failure(Exception.Message)
{
  public FatalFailure(FatalFailure failure)
    : base(failure)
  {
    ArgumentNullException.ThrowIfNull(failure);
    Exception = failure.Exception;
  }

  public Exception Exception { get; } = Exception;
}
