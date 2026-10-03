namespace ExpenseExplorer.Application.Dictionaries;

/// <summary>
/// Names that were renamed in the dictionaries, so an import that still reads the old name
/// gets the new one. Matching ignores letter case, since receipts print names in capitals or not.
/// </summary>
public sealed class NameAliases
{
    public static readonly NameAliases None = new([]);

    private readonly Dictionary<(NameKind Kind, string Alias), string> _names;

    public NameAliases(IEnumerable<(NameKind Kind, string Alias, string Name)> aliases) =>
        _names = aliases.ToDictionary(alias => (alias.Kind, Key(alias.Alias)), alias => alias.Name);

    /// <summary>The current name for <paramref name="name"/>, or the name itself when it was never renamed.</summary>
    public string Resolve(NameKind kind, string name) =>
        _names.TryGetValue((kind, Key(name)), out string? current) ? current : name;

    /// <summary>How an alias is stored and compared.</summary>
    public static string Key(string name) => name.Trim().ToUpperInvariant();
}
