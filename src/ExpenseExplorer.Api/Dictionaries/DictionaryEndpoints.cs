using ExpenseExplorer.Api.Auth;
using ExpenseExplorer.Api.Http;
using ExpenseExplorer.Application.Dictionaries;
using ExpenseExplorer.Contracts.Dictionaries;
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

        api.MapGet("/dictionaries/{kind}", UsageAsync).WithTags("Dictionaries");
        api.MapPost("/dictionaries/{kind}/rename", RenameAsync)
            .RequireAuthorization(Policies.CanEdit)
            .WithTags("Dictionaries");

        return api;
    }

    private static Task<IResult> RespondAsync(
        string? search,
        int? limit,
        Func<DictionaryQuery, Task<IReadOnlyList<string>>> run) =>
        Query(search, limit)
            .ToHttpAsync(
                async query => Result.Success(await run(query)),
                names => Results.Ok(names));

    private static Task<IResult> UsageAsync(
        string kind,
        string? search,
        int? limit,
        IDictionaryEditor editor,
        CancellationToken cancellationToken) =>
        ResultCombine.Combine(DictionaryRequestParser.ParseKind(kind), Query(search, limit), (nameKind, query) => (nameKind, query))
            .ToHttpAsync(
                async request => Result.Success(await editor.UsageAsync(request.nameKind, request.query, cancellationToken)),
                names => Results.Ok(names.Select(name => new NameUsageResponse(name.Name, name.Uses, name.LastUsed))));

    private static Task<IResult> RenameAsync(
        string kind,
        RenameNameRequest request,
        IDictionaryEditor editor,
        CancellationToken cancellationToken) =>
        DictionaryRequestParser.ParseRename(kind, request)
            .ToHttpAsync(
                command => DictionaryUseCases.RenameAsync(editor, command, cancellationToken),
                renamed => Results.Ok(new RenameNameResponse(renamed.Name, renamed.Changed, renamed.Merged)));

    private static Result<DictionaryQuery> Query(string? search, int? limit) =>
        Input.InRange(limit, DictionaryQuery.DefaultLimit, 1, DictionaryQuery.MaxLimit)
            .ForTarget("limit")
            .Map(validLimit => new DictionaryQuery(search?.Trim() ?? "", validLimit));
}
