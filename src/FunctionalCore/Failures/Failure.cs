namespace FunctionalCore.Failures;

public abstract record Failure(string Message)
{
  public static Failure Fatal(Exception ex) => new FatalFailure(ex);

  public static Failure NotFound(string message, string id) => new NotFoundFailure(message, id);

  public static Failure Validation(IEnumerable<ValidationError> errors) => new ValidationFailure(errors);

  public static Failure Validation(ValidationError error) => new ValidationFailure([error]);

  public static Failure Validation(string key, string message)
    => new ValidationFailure([ValidationError.Create(key, message)]);
}
