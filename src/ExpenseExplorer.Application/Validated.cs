using System.Collections.Immutable;
using System.Diagnostics;

namespace ExpenseExplorer.Application;

public sealed class Validated<T>
{
    private readonly IValidated _validated;
    private Validated(IValidated validated) => _validated = validated;

    public TResult Match<TResult>(
        Func<IEnumerable<ValidationError>, TResult> errors,
        Func<T, TResult> value)
    {
        ArgumentNullException.ThrowIfNull(errors);
        ArgumentNullException.ThrowIfNull(value);

        return _validated switch
        {
            Failure f => errors(f.Errors),
            Success s => value(s.Value),
            _ => throw new UnreachableException()
        };
    }

    internal static Validated<T> Succeed(T value) => new(new Success(value));
    internal static Validated<T> Failed(IEnumerable<ValidationError> errors) => new(new Failure([..errors]));

    private interface IValidated;
    private sealed record Success(T Value) : IValidated;
    private sealed record Failure(ImmutableArray<ValidationError> Errors) : IValidated;
}