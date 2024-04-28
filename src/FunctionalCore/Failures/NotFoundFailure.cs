namespace FunctionalCore.Failures;

public record NotFoundFailure(string Message, string Id)
  : Failure(Message)
{
  public NotFoundFailure(NotFoundFailure failure)
    : base(failure)
  {
    ArgumentNullException.ThrowIfNull(failure);
    Id = failure.Id;
  }
}
