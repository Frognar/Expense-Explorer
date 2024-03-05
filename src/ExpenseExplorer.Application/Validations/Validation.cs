namespace ExpenseExplorer.Application.Validations;

public static class Validation {
  public static Validated<S> Succeeded<S>(S value) => Validated<S>.Success(value);
  public static Validated<S> Failed<S>(IEnumerable<ValidationError> errors) => Validated<S>.Fail(errors);

  public static Validated<S> Apply<T, S>(this Func<T, S> map, Validated<T> source) {
    return Apply(Succeeded(map), source);
  }

  public static Validated<S> Apply<T, S>(this Validated<Func<T, S>> selector, Validated<T> source) {
    return selector.Match(
      _ => throw new NotImplementedException(),
      onSuccess: map => source.Match(
        onFailure: Failed<S>,
        value => Succeeded(map(value))
      )
    );
  }
}
