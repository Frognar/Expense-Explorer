namespace ExpenseExplorer.Application;

public static class FunctionalConversions
{
    public static Validated<T> ToValidated<T>(this Option<T> source, Func<ValidationError> onNone)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.Match(
            none: () => Validation.Failed<T>([onNone()]),
            some: Validation.Succeed);
    }

    public static Result<T, IEnumerable<ValidationError>> ToResult<T>(this Validated<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.Match(
            errors: Result.Failure<T, IEnumerable<ValidationError>>,
            value: Result.Success<T, IEnumerable<ValidationError>>);
    }
}