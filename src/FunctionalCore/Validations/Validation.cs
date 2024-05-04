namespace FunctionalCore.Validations;

using FunctionalCore.Failures;
using FunctionalCore.Monads;

public static class Validation
{
  public static Validated<S> Succeeded<S>(S value) => Validated<S>.Success(value);

  public static Validated<S> Failed<S>(IEnumerable<ValidationError> errors) => Validated<S>.Fail(errors);

  public static Either<Failure, S> ToEither<S>(this Validated<S> validated)
  {
    ArgumentNullException.ThrowIfNull(validated);
    return validated.Match(errors => Left.From<Failure, S>(Failure.Validation(errors)), Right.From<Failure, S>);
  }

  public static Result<S> ToResult<S>(this Validated<S> validated)
  {
    ArgumentNullException.ThrowIfNull(validated);
    return validated.Match(errors => Fail.OfType<S>(Failure.Validation(errors)), Success.From);
  }

  public static Validated<S> Apply<T, S>(this Func<T, S> map, Validated<T> source)
  {
    return Apply(Succeeded(map), source);
  }

  public static Validated<S> Apply<T, S>(this Validated<Func<T, S>> selector, Validated<T> source)
  {
    ArgumentNullException.ThrowIfNull(selector);
    return selector.Match(
      errors => source.Match(
        otherErrors => Failed<S>(errors.Concat(otherErrors)),
        _ => Failed<S>(errors)),
      map => source.Match(
        Failed<S>,
        value => Succeeded(map(value))));
  }

  public static Validated<Func<T1, S>> Apply<T, T1, S>(this Func<T, T1, S> map, Validated<T> source)
  {
    return Apply(Succeeded(map), source);
  }

  public static Validated<Func<T1, S>> Apply<T, T1, S>(this Validated<Func<T, T1, S>> selector, Validated<T> source)
  {
    ArgumentNullException.ThrowIfNull(selector);
    return selector.Match(
      errors => source.Match(
        otherErrors => Failed<Func<T1, S>>(errors.Concat(otherErrors)),
        _ => Failed<Func<T1, S>>(errors)),
      map => source.Match(
        Failed<Func<T1, S>>,
        a => Succeeded<Func<T1, S>>(b => map(a, b))));
  }

  public static Validated<Func<T1, T2, S>> Apply<T, T1, T2, S>(this Func<T, T1, T2, S> map, Validated<T> source)
  {
    return Apply(Succeeded(map), source);
  }

  public static Validated<Func<T1, T2, S>> Apply<T, T1, T2, S>(
    this Validated<Func<T, T1, T2, S>> selector,
    Validated<T> source)
  {
    ArgumentNullException.ThrowIfNull(selector);
    return selector.Match(
      errors => source.Match(
        otherErrors => Failed<Func<T1, T2, S>>(errors.Concat(otherErrors)),
        _ => Failed<Func<T1, T2, S>>(errors)),
      map => source.Match(
        Failed<Func<T1, T2, S>>,
        a => Succeeded<Func<T1, T2, S>>((b, c) => map(a, b, c))));
  }

  public static Validated<Func<T1, T2, T3, S>> Apply<T, T1, T2, T3, S>(
    this Func<T, T1, T2, T3, S> map,
    Validated<T> source)
  {
    return Apply(Succeeded(map), source);
  }

  public static Validated<Func<T1, T2, T3, S>> Apply<T, T1, T2, T3, S>(
    this Validated<Func<T, T1, T2, T3, S>> selector,
    Validated<T> source)
  {
    ArgumentNullException.ThrowIfNull(selector);
    return selector.Match(
      errors => source.Match(
        otherErrors => Failed<Func<T1, T2, T3, S>>(errors.Concat(otherErrors)),
        _ => Failed<Func<T1, T2, T3, S>>(errors)),
      map => source.Match(
        Failed<Func<T1, T2, T3, S>>,
        a => Succeeded<Func<T1, T2, T3, S>>((b, c, d) => map(a, b, c, d))));
  }

  public static Validated<Func<T1, T2, T3, T4, S>> Apply<T, T1, T2, T3, T4, S>(
    this Func<T, T1, T2, T3, T4, S> map,
    Validated<T> source)
  {
    return Apply(Succeeded(map), source);
  }

  public static Validated<Func<T1, T2, T3, T4, S>> Apply<T, T1, T2, T3, T4, S>(
    this Validated<Func<T, T1, T2, T3, T4, S>> selector,
    Validated<T> source)
  {
    ArgumentNullException.ThrowIfNull(selector);
    return selector.Match(
      errors => source.Match(
        otherErrors => Failed<Func<T1, T2, T3, T4, S>>(errors.Concat(otherErrors)),
        _ => Failed<Func<T1, T2, T3, T4, S>>(errors)),
      map => source.Match(
        Failed<Func<T1, T2, T3, T4, S>>,
        a => Succeeded<Func<T1, T2, T3, T4, S>>((b, c, d, e) => map(a, b, c, d, e))));
  }

  public static Validated<Func<T1, T2, T3, T4, T5, S>> Apply<T, T1, T2, T3, T4, T5, S>(
    this Func<T, T1, T2, T3, T4, T5, S> map,
    Validated<T> source)
  {
    return Apply(Succeeded(map), source);
  }

  public static Validated<Func<T1, T2, T3, T4, T5, S>> Apply<T, T1, T2, T3, T4, T5, S>(
    this Validated<Func<T, T1, T2, T3, T4, T5, S>> selector,
    Validated<T> source)
  {
    ArgumentNullException.ThrowIfNull(selector);
    return selector.Match(
      errors => source.Match(
        otherErrors => Failed<Func<T1, T2, T3, T4, T5, S>>(errors.Concat(otherErrors)),
        _ => Failed<Func<T1, T2, T3, T4, T5, S>>(errors)),
      map => source.Match(
        Failed<Func<T1, T2, T3, T4, T5, S>>,
        a => Succeeded<Func<T1, T2, T3, T4, T5, S>>((b, c, d, e, f) => map(a, b, c, d, e, f))));
  }

  public static Validated<Func<T1, T2, T3, T4, T5, T6, S>> Apply<T, T1, T2, T3, T4, T5, T6, S>(
    this Func<T, T1, T2, T3, T4, T5, T6, S> map,
    Validated<T> source)
  {
    return Apply(Succeeded(map), source);
  }

  public static Validated<Func<T1, T2, T3, T4, T5, T6, S>> Apply<T, T1, T2, T3, T4, T5, T6, S>(
    this Validated<Func<T, T1, T2, T3, T4, T5, T6, S>> selector,
    Validated<T> source)
  {
    ArgumentNullException.ThrowIfNull(selector);
    return selector.Match(
      errors => source.Match(
        otherErrors => Failed<Func<T1, T2, T3, T4, T5, T6, S>>(errors.Concat(otherErrors)),
        _ => Failed<Func<T1, T2, T3, T4, T5, T6, S>>(errors)),
      map => source.Match(
        Failed<Func<T1, T2, T3, T4, T5, T6, S>>,
        a => Succeeded<Func<T1, T2, T3, T4, T5, T6, S>>((b, c, d, e, f, g) => map(a, b, c, d, e, f, g))));
  }

  public static Validated<Func<T1, T2, T3, T4, T5, T6, T7, S>> Apply<T, T1, T2, T3, T4, T5, T6, T7, S>(
    this Func<T, T1, T2, T3, T4, T5, T6, T7, S> map,
    Validated<T> source)
  {
    return Apply(Succeeded(map), source);
  }

  public static Validated<Func<T1, T2, T3, T4, T5, T6, T7, S>> Apply<T, T1, T2, T3, T4, T5, T6, T7, S>(
    this Validated<Func<T, T1, T2, T3, T4, T5, T6, T7, S>> selector,
    Validated<T> source)
  {
    ArgumentNullException.ThrowIfNull(selector);
    return selector.Match(
      errors => source.Match(
        otherErrors => Failed<Func<T1, T2, T3, T4, T5, T6, T7, S>>(errors.Concat(otherErrors)),
        _ => Failed<Func<T1, T2, T3, T4, T5, T6, T7, S>>(errors)),
      map => source.Match(
        Failed<Func<T1, T2, T3, T4, T5, T6, T7, S>>,
        a => Succeeded<Func<T1, T2, T3, T4, T5, T6, T7, S>>((b, c, d, e, f, g, h) => map(a, b, c, d, e, f, g, h))));
  }

  public static Validated<Func<T1, T2, T3, T4, T5, T6, T7, T8, S>> Apply<T, T1, T2, T3, T4, T5, T6, T7, T8, S>(
    this Func<T, T1, T2, T3, T4, T5, T6, T7, T8, S> map,
    Validated<T> source)
  {
    return Apply(Succeeded(map), source);
  }

  public static Validated<Func<T1, T2, T3, T4, T5, T6, T7, T8, S>> Apply<T, T1, T2, T3, T4, T5, T6, T7, T8, S>(
    this Validated<Func<T, T1, T2, T3, T4, T5, T6, T7, T8, S>> selector,
    Validated<T> source)
  {
    ArgumentNullException.ThrowIfNull(selector);
    return selector.Match(
      errors => source.Match(
        otherErrors => Failed<Func<T1, T2, T3, T4, T5, T6, T7, T8, S>>(errors.Concat(otherErrors)),
        _ => Failed<Func<T1, T2, T3, T4, T5, T6, T7, T8, S>>(errors)),
      map => source.Match(
        Failed<Func<T1, T2, T3, T4, T5, T6, T7, T8, S>>,
        a => Succeeded<Func<T1, T2, T3, T4, T5, T6, T7, T8, S>>(
          (b, c, d, e, f, g, h, i) => map(a, b, c, d, e, f, g, h, i))));
  }

  public static Validated<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, S>> Apply<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, S>(
    this Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, S> map,
    Validated<T> source)
  {
    return Apply(Succeeded(map), source);
  }

  public static Validated<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, S>> Apply<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, S>(
    this Validated<Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, S>> selector,
    Validated<T> source)
  {
    ArgumentNullException.ThrowIfNull(selector);
    return selector.Match(
      errors => source.Match(
        otherErrors => Failed<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, S>>(errors.Concat(otherErrors)),
        _ => Failed<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, S>>(errors)),
      map => source.Match(
        Failed<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, S>>,
        a => Succeeded<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, S>>(
          (b, c, d, e, f, g, h, i, j) => map(a, b, c, d, e, f, g, h, i, j))));
  }
}
