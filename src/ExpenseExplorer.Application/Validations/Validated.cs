namespace ExpenseExplorer.Application.Validations;

using ExpenseExplorer.Application.Errors;

public class Validated<S>
{
  private readonly IValidation validation;

  private Validated(IValidation validation)
  {
    this.validation = validation;
  }

  private interface IValidation
  {
    public T Match<T>(Func<ValidationFailure, T> onFailure, Func<S, T> onSuccess);
  }

  public bool IsValid => validation.Match(_ => false, _ => true);

  public Validated<T> Map<T>(Func<S, T> map) => Match(Validated<T>.Fail, value => Validated<T>.Success(map(value)));

  public Validated<T> Select<T>(Func<S, T> selector) => Map(selector);

  public T Match<T>(Func<ValidationFailure, T> onFailure, Func<S, T> onSuccess)
    => validation.Match(onFailure, onSuccess);

  internal static Validated<S> Success(S value) => new(new Succeeded(value));

  internal static Validated<S> Fail(ValidationFailure errors) => new(new Failed(errors));

  private readonly record struct Succeeded(S Value) : IValidation
  {
    public S Value { get; } = Value;

    public T Match<T>(Func<ValidationFailure, T> onFailure, Func<S, T> onSuccess) => onSuccess(Value);
  }

  private readonly record struct Failed(ValidationFailure Errors) : IValidation
  {
    public ValidationFailure Errors { get; } = Errors;

    public T Match<T>(Func<ValidationFailure, T> onFailure, Func<S, T> onSuccess) => onFailure(Errors);
  }
}
