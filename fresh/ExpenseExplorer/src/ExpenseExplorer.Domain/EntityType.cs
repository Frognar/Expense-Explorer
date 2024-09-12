using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain;

public abstract record EntityType(
  UnsavedChangesType UnsavedChanges,
  VersionType Version);
