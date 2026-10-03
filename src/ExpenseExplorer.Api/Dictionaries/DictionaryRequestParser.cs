using ExpenseExplorer.Api.Http;
using ExpenseExplorer.Application.Dictionaries;
using ExpenseExplorer.Contracts.Dictionaries;
using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Api.Dictionaries;

/// <summary>Pure functions from raw dictionary requests to commands.</summary>
internal static class DictionaryRequestParser
{
    public static readonly Error UnknownKind = new(
        "Dictionary.UnknownKind", "Use stores, items or categories.", ErrorType.NotFound, Target: "kind");

    public static Result<NameKind> ParseKind(string kind) => kind switch
    {
        "stores" => Result.Success(NameKind.Store),
        "items" => Result.Success(NameKind.Item),
        "categories" => Result.Success(NameKind.Category),
        _ => Result.Failure<NameKind>(UnknownKind),
    };

    /// <summary>The new name follows the same rules as on a receipt; the old one only has to be given.</summary>
    public static Result<RenameName> ParseRename(string kind, RenameNameRequest request) =>
        ParseKind(kind).Bind(nameKind => ResultCombine.Combine(
                Input.Required(request.From).Map(from => from.Trim()).ForTarget("from"),
                NewName(nameKind, request.To).ForTarget("to"),
                (from, to) => new RenameName(nameKind, from, to))
            .Bind(command => command.From == command.To
                ? Result.Failure<RenameName>(new Error("Dictionary.SameName", "The new name is the same as the old one.", Target: "to"))
                : Result.Success(command)));

    private static Result<string> NewName(NameKind kind, string? name) => kind switch
    {
        NameKind.Store => StoreName.Create(name).Map(valid => valid.Value),
        NameKind.Item => ItemName.Create(name).Map(valid => valid.Value),
        _ => CategoryName.Create(name).Map(valid => valid.Value),
    };
}
