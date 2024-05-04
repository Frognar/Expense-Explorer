namespace FunctionalCore.Failures;

public static class FailureExtensions
{
  public static TResult Match<TResult>(
    this Failure failure,
    Func<string, Exception, TResult> fatal,
    Func<string, string, TResult> notFound,
    Func<string, IEnumerable<ValidationError>, TResult> validation)
  {
    ArgumentNullException.ThrowIfNull(fatal);
    ArgumentNullException.ThrowIfNull(notFound);
    ArgumentNullException.ThrowIfNull(validation);

    return failure switch
    {
      FatalFailure f => fatal(f.Message, f.Exception),
      NotFoundFailure nf => notFound(nf.Message, nf.Id),
      ValidationFailure vf => validation(vf.Message, vf.Errors),
      _ => throw new InvalidOperationException("Unknown failure type"),
    };
  }
}
