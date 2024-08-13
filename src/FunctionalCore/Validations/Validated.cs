using FunctionalCore.Failures;

namespace FunctionalCore.Validations;

public class Validated<S>
{
  private readonly IValidation _validation;

  private Validated(IValidation validation)
  {
    _validation = validation;
  }

  private interface IValidation
  {
    public T Match<T>(Func<IEnumerable<ValidationError>, T> onFailure, Func<S, T> onSuccess);
  }

  public bool IsValid => _validation.Match(_ => false, _ => true);

  public Validated<T> Map<T>(Func<S, T> map) => Match(Validated<T>.Fail, value => Validated<T>.Success(map(value)));

  public Validated<T> Select<T>(Func<S, T> selector) => Map(selector);

  public T Match<T>(Func<IEnumerable<ValidationError>, T> onFailure, Func<S, T> onSuccess)
    => _validation.Match(onFailure, onSuccess);

  internal static Validated<S> Success(S value) => new(new Succeeded(value));

  internal static Validated<S> Fail(IEnumerable<ValidationError> errors) => new(new Failed(errors));

  private readonly record struct Succeeded(S Value) : IValidation
  {
    public S Value { get; } = Value;

    public T Match<T>(Func<IEnumerable<ValidationError>, T> onFailure, Func<S, T> onSuccess) => onSuccess(Value);
  }

  private readonly record struct Failed(IEnumerable<ValidationError> Errors) : IValidation
  {
    public IEnumerable<ValidationError> Errors { get; } = Errors;

    public T Match<T>(Func<IEnumerable<ValidationError>, T> onFailure, Func<S, T> onSuccess) => onFailure(Errors);
  }
}
