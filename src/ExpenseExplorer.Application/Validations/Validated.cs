namespace ExpenseExplorer.Application.Validations;

public class Validated<S> {
  private Validated(S value) {
    IsValid = true;
  }

  private Validated(IEnumerable<ValidationError> errors) {
    IsValid = false;
  }

  public bool IsValid { get; init; }

  internal static Validated<S> Success(S value) => new(value);
  internal static Validated<S> Fail(IEnumerable<ValidationError> errors) => new(errors);
}
