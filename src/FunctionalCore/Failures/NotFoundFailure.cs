namespace FunctionalCore.Failures;

public record NotFoundFailure(string Message, string Id) : Failure(Message);
