namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct Id {
  public Id() {
    Value = Guid.NewGuid().ToString("N");
  }

  public Id(string value) {
    ArgumentNullException.ThrowIfNull(value);
    Value = value.Trim();
  }

  public string Value { get; }

  public static Id Unique() {
    return new Id();
  }
}
