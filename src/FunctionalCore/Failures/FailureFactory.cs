namespace FunctionalCore.Failures;

using DotResult;

public static class FailureFactory
{
  public static Failure Fatal(Exception ex)
  {
    ArgumentNullException.ThrowIfNull(ex);
    return Failure.Fatal(
      message: ex.Message,
      metadata: new Dictionary<string, object> { { "Exception", ex } });
  }

  public static Failure NotFound(string message, string id)
  {
    return Failure.NotFound(
      message: message,
      metadata: new Dictionary<string, object> { { "Id", id } });
  }

  public static Failure Validation(ValidationError error)
  {
    IEnumerable<ValidationError> errors = [error];
    return Failure.Validation(metadata: new Dictionary<string, object> { { "Errors", errors } });
  }
}
