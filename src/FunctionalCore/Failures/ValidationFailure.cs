namespace FunctionalCore.Failures;

public record ValidationFailure(IEnumerable<ValidationError> Errors)
  : Failure("One or more validation errors occurred.")
{
  public ValidationFailure Concat(ValidationFailure other)
  {
    ArgumentNullException.ThrowIfNull(other);
    return Validation(Errors.Concat(other.Errors));
  }
}
