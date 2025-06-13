using DotMaybe;
using DotResult;

namespace ExpenseExplorer.Application;

public static class FunctionalConversions
{
    public static Validated<T> ToValidated<T>(this Maybe<T> source, Func<ValidationError> onNone) =>
        source.Match(
            none: () => Validation.Failed<T>([onNone()]),
            some: Validation.Succeed);

    public static Result<T> ToResult<T>(this Maybe<T> source, Func<Failure> onNone) =>
        source.Match(
            none: () => Fail.OfType<T>(onNone()),
            some: Success.From);

    public static Result<T> ToResult<T>(this Validated<T> source) =>
        source.Match(
            errors: errors => Fail.OfType<T>(
                errors.Select(e => Failure.Validation(e.PropertyName, e.Error))),
            value: Success.From);
}