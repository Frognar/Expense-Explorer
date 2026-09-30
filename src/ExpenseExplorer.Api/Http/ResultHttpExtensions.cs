using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Api.Http;

internal static class ResultHttpExtensions
{
    /// <summary>
    /// Runs <paramref name="handle"/> only for valid input. Input errors and errors from the
    /// use case both end up as ProblemDetails; a success is shaped by <paramref name="onSuccess"/>.
    /// </summary>
    public static Task<IResult> ToHttpAsync<TInput, TOutput>(
        this Result<TInput> input,
        Func<TInput, Task<Result<TOutput>>> handle,
        Func<TOutput, IResult> onSuccess) =>
        input.Match(
            async valid => (await handle(valid)).Match(onSuccess, ErrorResults.From),
            errors => Task.FromResult(ErrorResults.From(errors)));

    public static IResult ToHttp<TInput>(this Result<TInput> input, Func<TInput, IResult> onSuccess) =>
        input.Match(onSuccess, ErrorResults.From);
}
