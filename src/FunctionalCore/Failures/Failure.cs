namespace FunctionalCore.Failures;

public sealed record Failure
{
  private readonly IFailure _failure;

  private Failure(IFailure failure)
  {
    _failure = failure;
  }

  private interface IFailure
  {
    TResult Match<TResult>(
      Func<string, Exception, TResult> fatal,
      Func<string, string, TResult> notFound,
      Func<string, IEnumerable<ValidationError>, TResult> validation);
  }

  public TResult Match<TResult>(
    Func<string, Exception, TResult> fatal,
    Func<string, string, TResult> notFound,
    Func<string, IEnumerable<ValidationError>, TResult> validation)
  {
    return _failure.Match(fatal, notFound, validation);
  }

  public static Failure Fatal(Exception ex) => new(new FatalFailure(ex));

  public static Failure NotFound(string message, string id) => new(new NotFoundFailure(message, id));

  public static Failure Validation(IEnumerable<ValidationError> errors) => new(new ValidationFailure(errors));

  public static Failure Validation(ValidationError error) => new(new ValidationFailure([error]));

  public static Failure Validation(string key, string message)
    => new(new ValidationFailure([ValidationError.Create(key, message)]));

  private sealed record FatalFailure(Exception Exception) : IFailure
  {
    public TResult Match<TResult>(
      Func<string, Exception, TResult> fatal,
      Func<string, string, TResult> notFound,
      Func<string, IEnumerable<ValidationError>, TResult> validation)
    {
      return fatal(Exception.Message, Exception);
    }
  }

  private sealed record NotFoundFailure(string Message, string Id) : IFailure
  {
    public TResult Match<TResult>(
      Func<string, Exception, TResult> fatal,
      Func<string, string, TResult> notFound,
      Func<string, IEnumerable<ValidationError>, TResult> validation)
    {
      return notFound(Message, Id);
    }
  }

  private sealed record ValidationFailure(IEnumerable<ValidationError> Errors) : IFailure
  {
    public TResult Match<TResult>(
      Func<string, Exception, TResult> fatal,
      Func<string, string, TResult> notFound,
      Func<string, IEnumerable<ValidationError>, TResult> validation)
    {
      return validation("One or more validation errors occurred.", Errors);
    }
  }
}
