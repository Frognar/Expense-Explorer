using System.Collections.Immutable;
using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Api.Http;

/// <summary>
/// Turns domain and input errors into ProblemDetails. Besides human readable messages,
/// every response carries <c>errorCodes</c> (input name → codes) for clients to localize.
/// </summary>
internal static class ErrorResults
{
    public static IResult From(ImmutableArray<Error> errors)
    {
        Dictionary<string, object?> extensions = new()
        {
            ["errorCodes"] = Group(errors, error => error.Code),
        };

        return errors switch
        {
            _ when errors.Any(error => error.Type == ErrorType.Unauthorized) => Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                detail: Describe(errors, ErrorType.Unauthorized),
                extensions: extensions),
            _ when errors.Any(error => error.Type == ErrorType.NotFound) => Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                detail: Describe(errors, ErrorType.NotFound),
                extensions: extensions),
            _ when errors.Any(error => error.Type == ErrorType.Conflict) => Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                detail: Describe(errors, ErrorType.Conflict),
                extensions: extensions),
            _ => Results.ValidationProblem(
                Group(errors, error => error.Message),
                extensions: extensions),
        };
    }

    private static Dictionary<string, string[]> Group(ImmutableArray<Error> errors, Func<Error, string> select) =>
        errors
            .GroupBy(error => error.Target ?? "")
            .ToDictionary(group => group.Key, group => group.Select(select).ToArray());

    private static string Describe(ImmutableArray<Error> errors, ErrorType type) =>
        string.Join(" ", errors.Where(error => error.Type == type).Select(error => error.Message));
}
