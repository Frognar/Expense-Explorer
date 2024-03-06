namespace ExpenseExplorer.Domain.ValueObjects;

public class Store {
  public Store(string name) {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);
    Name = name;
  }

  public string Name { get; }
}
