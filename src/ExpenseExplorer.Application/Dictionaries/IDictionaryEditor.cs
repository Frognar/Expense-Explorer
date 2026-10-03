namespace ExpenseExplorer.Application.Dictionaries;

public interface IDictionaryEditor
{
    /// <summary>
    /// Replaces the name everywhere and remembers the old one as an alias, in one transaction.
    /// Returns <c>null</c> when no receipt uses <see cref="RenameName.From"/>.
    /// </summary>
    Task<RenamedName?> RenameAsync(RenameName command, CancellationToken cancellationToken);

    /// <summary>Names in use with how often and how recently, for tidying the dictionaries.</summary>
    Task<IReadOnlyList<NameUsage>> UsageAsync(NameKind kind, DictionaryQuery query, CancellationToken cancellationToken);
}

public sealed record NameUsage(string Name, int Uses, DateOnly LastUsed);
