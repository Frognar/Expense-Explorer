using ExpenseExplorer.Api.Http;
using ExpenseExplorer.Application.Dictionaries;
using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Api.Dictionaries;

internal static class DictionaryEndpoints
{
    public static RouteGroupBuilder MapDictionaries(this RouteGroupBuilder api)
    {
        api.MapGet("/stores", (string? search, int? limit, IDictionaryQueries queries, CancellationToken ct) =>
                RespondAsync(search, limit, query => queries.StoresAsync(query, ct)))
            .WithTags("Dictionaries");

        api.MapGet("/items", (string? search, int? limit, IDictionaryQueries queries, CancellationToken ct) =>
                RespondAsync(search, limit, query => queries.ItemsAsync(query, ct)))
            .WithTags("Dictionaries");

        api.MapGet("/categories", (string? search, int? limit, IDictionaryQueries queries, CancellationToken ct) =>
                RespondAsync(search, limit, query => queries.CategoriesAsync(query, ct)))
            .WithTags("Dictionaries");

        return api;
    }

    private static Task<IResult> RespondAsync(
        string? search,
        int? limit,
        Func<DictionaryQuery, Task<IReadOnlyList<string>>> run) =>
        Input.InRange(limit, DictionaryQuery.DefaultLimit, 1, DictionaryQuery.MaxLimit)
            .ForTarget("limit")
            .Map(validLimit => new DictionaryQuery(search?.Trim() ?? "", validLimit))
            .ToHttpAsync(
                async query => Result.Success(await run(query)),
                names => Results.Ok(names));
}
