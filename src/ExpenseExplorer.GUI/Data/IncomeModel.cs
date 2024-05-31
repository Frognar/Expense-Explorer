namespace ExpenseExplorer.GUI.Data;

public sealed class IncomeModel
{
  public string Id { get; set; } = string.Empty;

  public string Source { get; set; } = string.Empty;

  public decimal Amount { get; set; }

  public string Category { get; set; } = string.Empty;

  public DateOnly ReceivedDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

  public string Description { get; set; } = string.Empty;

  public IncomeModel MakeCopy()
  {
    return new IncomeModel
    {
      Id = Id,
      Source = Source,
      Amount = Amount,
      Category = Category,
      ReceivedDate = ReceivedDate,
      Description = Description,
    };
  }

  public void CopyFrom(IncomeModel income)
  {
    ArgumentNullException.ThrowIfNull(income);
    Source = income.Source;
    Amount = income.Amount;
    Category = income.Category;
    ReceivedDate = income.ReceivedDate;
    Description = income.Description;
  }
}
