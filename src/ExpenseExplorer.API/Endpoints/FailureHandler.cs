using System.Net;
using DotResult;
using FunctionalCore.Failures;

namespace ExpenseExplorer.API.Endpoints;

internal static class FailureHandler
{
  public static IResult Handle(Failure failure)
  {
    ArgumentNullException.ThrowIfNull(failure);
    return failure.Code switch
    {
      "General.Fatal" => HandleFatal(failure.Message),
      "General.NotFound" => HandleNotFound(failure.Message, (string)failure.Metadata.GetValueOrDefault("Id", string.Empty)),
      "General.Validation" => HandleValidation(failure.Message, (IEnumerable<ValidationError>)failure.Metadata.GetValueOrDefault("Errors", Enumerable.Empty<ValidationError>())),
      _ => Results.Problem(),
    };
  }

  private static IResult HandleFatal(string message)
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
