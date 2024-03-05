namespace ExpenseExplorer.Application.Validations;

public static class Validation {
  public static Validated<S> Succeeded<S>(S value) => Validated<S>.Success(value);
  public static Validated<S> Failed<S>(IEnumerable<ValidationError> errors) => Validated<S>.Fail(errors);
}
