using System.Diagnostics;
using FunctionalCore.Failures;

namespace ExpenseExplorer.Tests.Common;

public static class FailureExtensions
{
  public static TResult Match<TResult>(
    this Failure failure,
    Func<string, Exception, TResult> fatal,
    Func<string, string, TResult> notFound,
    Func<string, IEnumerable<ValidationError>, TResult> validation)
  {
    return failure.Code switch
    {
      "General.Fatal" => fatal(failure.Message, (Exception)failure.Metadata.GetValueOrDefault("Exception", new UnreachableException())),
      "General.NotFound" => notFound(failure.Message, (string)failure.Metadata.GetValueOrDefault("Id", string.Empty)),
      "General.Validation" => validation(failure.Message, (IEnumerable<ValidationError>)failure.Metadata.GetValueOrDefault("Errors", Enumerable.Empty<ValidationError>())),
      _ => throw new UnreachableException(),
    };
  }
}
