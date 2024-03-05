namespace ExpenseExplorer.Application.Validations;

public class Validated<S> {
  private readonly Validation validation;

  private Validated(Validation validation) {
    this.validation = validation;
  }

  public bool IsValid => validation.Match(_ => false, _ => true);

  public T Match<T>(Func<IEnumerable<ValidationError>, T> onFailure, Func<S, T> onSuccess)
    => validation.Match(onFailure, onSuccess);

  internal static Validated<S> Success(S value) => new(new Succeeded(value));
  internal static Validated<S> Fail(IEnumerable<ValidationError> errors) => new(new Failed(errors));

  private interface Validation {
    public T Match<T>(Func<IEnumerable<ValidationError>, T> onFailure, Func<S, T> onSuccess);
  }

  private readonly record struct Succeeded(S Value) : Validation {
    public S Value { get; } = Value;
    public T Match<T>(Func<IEnumerable<ValidationError>, T> onFailure, Func<S, T> onSuccess) => onSuccess(Value);
  }

  private readonly record struct Failed(IEnumerable<ValidationError> Errors) : Validation {
    public IEnumerable<ValidationError> Errors { get; } = Errors;
    public T Match<T>(Func<IEnumerable<ValidationError>, T> onFailure, Func<S, T> onSuccess) => onFailure(Errors);
  }
}
