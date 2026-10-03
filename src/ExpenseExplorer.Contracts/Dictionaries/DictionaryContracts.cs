namespace ExpenseExplorer.Contracts.Dictionaries;

/// <summary>A store, item or category name with how many receipts (stores) or lines use it.</summary>
public sealed record NameUsageResponse(string Name, int Uses, DateOnly LastUsed);

/// <summary>
/// Body of <c>POST /api/v1/dictionaries/{kind}/rename</c>. Renaming to a name already in use
/// merges the two; later imports that read the old name get the new one.
/// </summary>
public sealed record RenameNameRequest(string? From, string? To);

/// <summary><see cref="Changed"/> counts receipts for a store and receipt lines for an item or category.</summary>
public sealed record RenameNameResponse(string Name, int Changed, bool Merged);
