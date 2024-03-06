namespace ExpenseExplorer.Domain.ValueObjects;

public record PurchaseDate {
  private PurchaseDate(DateOnly date, DateOnly today) {
    if (date > today) {
      throw new ArgumentException("Purchase date cannot be in the future");
    }

    Date = date;
  }

  public DateOnly Date { get; }

  public static PurchaseDate Create(DateOnly date, DateOnly today) {
    return new PurchaseDate(date, today);
  }
}
