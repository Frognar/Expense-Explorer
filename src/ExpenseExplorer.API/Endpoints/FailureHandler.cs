namespace ExpenseExplorer.API.Endpoints;

using System.Net;
using FunctionalCore.Failures;

internal static class FailureHandler
{
  public static IResult Handle(Failure failure)
  {
    ArgumentNullException.ThrowIfNull(failure);
    return failure.Match(HandleFatal, HandleNotFound, HandleValidation);
  }

  private static IResult HandleFatal(string message, Exception ex)
  {
    return Results.Problem(detail: message, statusCode: (int)HttpStatusCode.InternalServerError);
  }

  private static IResult HandleValidation(string message, IEnumerable<ValidationError> errors)
  {
    return Results.Problem(
      detail: message,
      statusCode: (int)HttpStatusCode.BadRequest,
      extensions: new Dictionary<string, object?>
      {
        ["Errors"] = errors
          .GroupBy(e => e.Property)
          .ToDictionary(
            e => e.Key,
            e => e.Select(m => m.ErrorCode)),
      });
  }

  private static IResult HandleNotFound(string message, string id)
  {
    return Results.Problem(
      detail: message,
      statusCode: (int)HttpStatusCode.NotFound,
      extensions: new Dictionary<string, object?> { ["Id"] = id, });
  }
}
