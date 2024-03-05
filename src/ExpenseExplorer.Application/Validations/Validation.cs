namespace ExpenseExplorer.Application.Validations;

public static class Validation {
  public static Validated<S> Succeeded<S>(S value) => Validated<S>.Success(value);
  public static Validated<S> Failed<S>(IEnumerable<ValidationError> errors) => Validated<S>.Fail(errors);

  public static Validated<S> Apply<T, S>(this Func<T, S> map, Validated<T> source) {
    return Apply(Succeeded(map), source);
  }

  public static Validated<S> Apply<T, S>(this Validated<Func<T, S>> selector, Validated<T> source) {
    return selector.Match(
      errors => source.Match(
        otherErrors => Failed<S>(errors.Concat(otherErrors)),
        _ => Failed<S>(errors)
      ),
      map => source.Match(
        Failed<S>,
        value => Succeeded(map(value))
      )
    );
  }

  public static Validated<Func<T1, S>> Apply<T, T1, S>(this Func<T, T1, S> map, Validated<T> source) {
    return Apply(Succeeded(map), source);
  }

  public static Validated<Func<T1, S>> Apply<T, T1, S>(this Validated<Func<T, T1, S>> selector, Validated<T> source) {
    return selector.Match(
      _ => throw new NotImplementedException(),
      map => source.Match(
        Failed<Func<T1, S>>,
        a => Succeeded<Func<T1, S>>(b => map(a, b))
      )
    );
  }
}
