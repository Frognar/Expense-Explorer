namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct Id {
  public Id(string value) {
    Value = value.Trim();
  }

  public string Value { get; }
}
