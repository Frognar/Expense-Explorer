namespace FunctionalCore.Failures;

public abstract record Failure(string Message)
{
  public string Message { get; } = Message;
}
