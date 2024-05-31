namespace ExpenseExplorer.GUI.Data;

using ExpenseExplorer.API.Contract.ReadModel;

public sealed class IncomeModel
{
  public string Id { get; set; } = string.Empty;

  public string Source { get; set; } = string.Empty;

  public decimal Amount { get; set; }

  public string Category { get; set; } = string.Empty;

  public DateOnly ReceivedDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

  public string Description { get; set; } = string.Empty;

  public static IncomeModel FromResponse(IncomeResponse response)
  {
    ArgumentNullException.ThrowIfNull(response);
    return new IncomeModel
    {
      Id = response.Id,
      Source = response.Source,
      Amount = response.Amount,
      Category = response.Category,
      ReceivedDate = response.ReceivedDate,
      Description = response.Description,
    };
  }
}
