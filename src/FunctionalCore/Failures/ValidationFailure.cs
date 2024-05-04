namespace FunctionalCore.Failures;

public record ValidationFailure(IEnumerable<ValidationError> Errors)
  : Failure("One or more validation errors occurred.");
