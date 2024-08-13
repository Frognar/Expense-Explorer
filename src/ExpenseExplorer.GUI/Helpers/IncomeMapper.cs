using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Contract.ReadModel;
using ExpenseExplorer.GUI.Data;

namespace ExpenseExplorer.GUI.Helpers;

public static class IncomeMapper
{
  public static IncomeModel ToViewModel(this IncomeResponse income)
  {
    ArgumentNullException.ThrowIfNull(income);
    return new IncomeModel
    {
      Id = income.Id,
      Source = income.Source,
      Amount = income.Amount,
      Category = income.Category,
      ReceivedDate = income.ReceivedDate,
      Description = income.Description,
    };
  }

  public static AddIncomeRequest ToAddRequest(this IncomeModel income)
  {
    ArgumentNullException.ThrowIfNull(income);
    return new AddIncomeRequest(income.Source, income.Amount, income.Category, income.ReceivedDate, income.Description);
  }

  public static UpdateIncomeDetailsRequest ToEditRequest(this IncomeModel income)
  {
    ArgumentNullException.ThrowIfNull(income);
    return new UpdateIncomeDetailsRequest(
      income.Source,
      income.Amount,
      income.Category,
      income.ReceivedDate,
      income.Description);
  }
}
