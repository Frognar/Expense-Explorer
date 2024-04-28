namespace FunctionalCore.Failures;

public abstract record Failure(string Message)
{
  public static FatalFailure Fatal(Exception ex) => new(ex);

  public static NotFoundFailure NotFound(string message, string id) => new(message, id);

  public static ValidationFailure Validation(IEnumerable<ValidationError> errors) => new(errors);

  public static ValidationFailure Validation(ValidationError error) => new([error]);

  public static ValidationFailure Validation(string key, string message) => new([ValidationError.Create(key, message)]);
}
