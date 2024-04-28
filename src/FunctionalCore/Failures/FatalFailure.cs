namespace FunctionalCore.Failures;

public record FatalFailure(Exception Exception) : Failure(Exception.Message);
