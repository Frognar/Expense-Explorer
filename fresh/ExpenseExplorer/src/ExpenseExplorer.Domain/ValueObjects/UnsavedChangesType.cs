using System.Collections.ObjectModel;
using ExpenseExplorer.Domain.Facts;

namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct UnsavedChangesType(IReadOnlyCollection<Fact> Facts);

public static class UnsavedChanges
{
  public static UnsavedChangesType Empty() => new([]);

  public static UnsavedChangesType New(params Fact[] facts) => new(facts);

  public static UnsavedChangesType Append(this UnsavedChangesType changes, Fact fact)
    => new(new ReadOnlyCollection<Fact>(changes.Facts.Append(fact).ToList()));
}
