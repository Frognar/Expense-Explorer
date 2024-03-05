namespace ExpenseExplorer.Application.Validations;

public class Validated<S> {
  public Validated(S value) {
    IsValid = true;
  }

  public Validated(IEnumerable<ValidationError> errors) {
    IsValid = false;
  }

  public bool IsValid { get; init; }
}
