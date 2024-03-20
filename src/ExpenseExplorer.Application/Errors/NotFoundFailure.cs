namespace ExpenseExplorer.Application.Errors;

using ExpenseExplorer.Domain.ValueObjects;

public record NotFoundFailure(string Message, Id Id) : Failure(Message);
