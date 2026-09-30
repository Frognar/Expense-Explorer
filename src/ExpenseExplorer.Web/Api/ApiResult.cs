using System.Net;

namespace ExpenseExplorer.Web.Api;

/// <summary>What the API answered: a value, or a problem with error codes per field.</summary>
public sealed class ApiResult<T>
{
    private readonly T? _value;

    private ApiResult(T? value, ApiProblem? problem)
    {
        _value = value;
        Problem = problem;
    }

    public ApiProblem? Problem { get; }

    public bool IsSuccess => Problem is null;

    public T Value => IsSuccess ? _value! : throw new InvalidOperationException("The call failed.");

    /// <summary>The value when the call succeeded, otherwise the default.</summary>
    public T? ValueOrDefault => _value;

    internal static ApiResult<T> Create(T? value, ApiProblem? problem) => new(value, problem);
}

public static class ApiResult
{
    public static ApiResult<T> Success<T>(T value) => ApiResult<T>.Create(value, null);

    public static ApiResult<T> Failure<T>(ApiProblem problem) => ApiResult<T>.Create(default, problem);
}

/// <summary>
/// A ProblemDetails response. <see cref="ErrorCodes"/> maps a field name to its error codes;
/// errors about the request as a whole use the empty field name.
/// </summary>
public sealed record ApiProblem(HttpStatusCode Status, IReadOnlyDictionary<string, string[]> ErrorCodes)
{
    public IReadOnlyList<string> CodesFor(string field) =>
        ErrorCodes.TryGetValue(field, out string[]? codes) ? codes : [];

    /// <summary>Codes not tied to any of the given fields, e.g. "not found" or an unexpected server error.</summary>
    public IReadOnlyList<string> CodesOutside(params string[] fields) =>
        [.. ErrorCodes.Where(entry => !fields.Contains(entry.Key, StringComparer.OrdinalIgnoreCase)).SelectMany(entry => entry.Value)];
}
