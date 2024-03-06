namespace ExpenseExplorer.Domain.ValueObjects;

public record PurchaseDate {
  private PurchaseDate(DateOnly date) {
    Date = date;
  }

  public DateOnly Date { get; }

  public static PurchaseDate Create(DateOnly date, DateOnly today) {
    return new PurchaseDate(date);
  }
}
