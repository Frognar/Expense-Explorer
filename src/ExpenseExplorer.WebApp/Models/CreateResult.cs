namespace ExpenseExplorer.WebApp.Models;

internal interface ICreateResult;
internal sealed record SuccessCreateResult(Guid Id): ICreateResult;
internal sealed record ErrorCreateResult(string Error) : ICreateResult;

internal static class CreateResult
{
    public static async Task SwitchAsync(
        this Task<ICreateResult> result,
        Action<SuccessCreateResult> onSuccess,
        Func<ErrorCreateResult, Task> onError)
    {
        switch (await result)
        {
            case SuccessCreateResult success:
                onSuccess(success);
                break;

            case ErrorCreateResult error:
                await onError(error);
                break;
        }
    }
}