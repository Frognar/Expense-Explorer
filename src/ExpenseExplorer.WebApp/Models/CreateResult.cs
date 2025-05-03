namespace ExpenseExplorer.WebApp.Models;

internal interface ICreateResult;
internal sealed record SuccessCreateResult(Guid Id): ICreateResult;
internal sealed record ErrorCreateResult(string Error) : ICreateResult;