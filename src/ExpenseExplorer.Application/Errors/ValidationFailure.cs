namespace ExpenseExplorer.Application.Errors;

public record ValidationFailure(IEnumerable<ValidationError> Errors)
  : Failure("One or more validation errors occurred.");
