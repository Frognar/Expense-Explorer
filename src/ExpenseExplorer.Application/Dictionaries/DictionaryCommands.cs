namespace ExpenseExplorer.Application.Dictionaries;

/// <summary>
/// Renames a store, item or category on every receipt. When <see cref="To"/> is already in use,
/// the two names become one. <see cref="To"/> has passed the rules of its kind's name;
/// <see cref="From"/> only has to match a name in use exactly.
/// </summary>
public sealed record RenameName(NameKind Kind, string From, string To);

/// <summary>How many receipt lines (or receipts, for a store) now carry the new name.</summary>
public sealed record RenamedName(string Name, int Changed, bool Merged);
