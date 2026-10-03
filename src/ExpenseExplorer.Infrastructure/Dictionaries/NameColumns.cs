using ExpenseExplorer.Application.Dictionaries;

namespace ExpenseExplorer.Infrastructure.Dictionaries;

/// <summary>How each kind of name is stored.</summary>
internal static class NameColumns
{
    public static string Code(NameKind kind) => kind switch
    {
        NameKind.Store => "store",
        NameKind.Item => "item",
        NameKind.Category => "category",
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
    };

    public static NameKind? Kind(string code) => code switch
    {
        "store" => NameKind.Store,
        "item" => NameKind.Item,
        "category" => NameKind.Category,
        _ => null,
    };
}
