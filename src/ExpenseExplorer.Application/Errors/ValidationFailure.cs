namespace ExpenseExplorer.Application.Errors;

public record ValidationFailure(IEnumerable<ValidationError> Errors)
  : Failure("One or more validation errors occurred.")
{
  public ValidationFailure Concat(ValidationFailure other)
  {
    ArgumentNullException.ThrowIfNull(other);
    return new ValidationFailure(Errors.Concat(other.Errors));
  }

  public static ValidationFailure SingleFailure(ValidationError error)
  {
    ArgumentNullException.ThrowIfNull(error);
    return new ValidationFailure([error]);
  }

  public static ValidationFailure SingleFailure(string key, string message)
  {
    ArgumentNullException.ThrowIfNull(key);
    ArgumentNullException.ThrowIfNull(message);
    return new ValidationFailure([ValidationError.Create(key, message)]);
  }
}
