using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Api.Http;

/// <summary>Small parsers for raw request values that are not domain concepts themselves.</summary>
internal static class Input
{
    public static Result<T> Required<T>(T? value)
        where T : struct =>
        value is { } present
            ? Result.Success(present)
            : Result.Failure<T>(new Error("Input.Required", "Value is required."));

    public static Result<string> Required(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? Result.Failure<string>(new Error("Input.Required", "Value is required."))
            : Result.Success(value);

    /// <summary>A missing value stays missing; a present one must parse.</summary>
    public static Result<TOut?> Optional<TIn, TOut>(TIn? value, Func<TIn, Result<TOut>> parse)
        where TIn : struct
        where TOut : class =>
        value is { } present
            ? parse(present).Map<TOut?>(parsed => parsed)
            : Result.Success<TOut?>(null);

    public static Result<TOut?> Optional<TOut>(string? value, Func<string, Result<TOut>> parse)
        where TOut : class =>
        value is null
            ? Result.Success<TOut?>(null)
            : parse(value).Map<TOut?>(parsed => parsed);

    public static Result<int> InRange(int? value, int defaultValue, int min, int max) =>
        (value ?? defaultValue) is var number && number >= min && number <= max
            ? Result.Success(number)
            : Result.Failure<int>(new Error("Input.OutOfRange", $"Value must be between {min} and {max}."));

    public static Result<(T? From, T? To)> OrderedRange<T>(T? from, T? to)
        where T : struct, IComparable<T> =>
        from is { } start && to is { } end && start.CompareTo(end) > 0
            ? Result.Failure<(T?, T?)>(new Error("Input.RangeReversed", "The start of the range is after its end."))
            : Result.Success((from, to));

    /// <summary>Trimmed values of a list filter; blanks and repeats are dropped.</summary>
    public static string[] NonBlankDistinct(IEnumerable<string>? values) =>
        [.. (values ?? []).Select(value => value.Trim()).Where(value => value.Length > 0).Distinct(StringComparer.Ordinal)];

    /// <summary>Trimmed text, or null when nothing but whitespace was given.</summary>
    public static string? NonBlank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public static DateOnly Today(this TimeProvider clock) => DateOnly.FromDateTime(clock.GetLocalNow().DateTime);
}
