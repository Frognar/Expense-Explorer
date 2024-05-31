namespace ExpenseExplorer.GUI.Data;

public sealed class IncomeModel
{
  public string Id { get; set; } = string.Empty;

  public string Source { get; set; } = string.Empty;

  public decimal Amount { get; set; }

  public string Category { get; set; } = string.Empty;

  public DateOnly ReceivedDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

  public string Description { get; set; } = string.Empty;
}
