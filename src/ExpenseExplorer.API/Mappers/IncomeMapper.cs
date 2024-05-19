namespace ExpenseExplorer.API.Mappers;

using ExpenseExplorer.API.Contract;
using ExpenseExplorer.Application.Incomes.Commands;
using ExpenseExplorer.Domain.Incomes;

public static class IncomeMapper
{
  public static AddIncomeCommand MapToCommand(this AddIncomeRequest request, DateOnly today)
  {
    ArgumentNullException.ThrowIfNull(request);
    return new AddIncomeCommand(request.Source, request.Amount, request.Category, request.ReceivedDate, request.Description, today);
  }

  public static UpdateIncomeDetailsCommand MapToCommand(this UpdateIncomeDetailsRequest request, DateOnly today)
  {
    ArgumentNullException.ThrowIfNull(request);
    return new UpdateIncomeDetailsCommand(request.IncomeId, request.Source, request.Amount, request.Category, request.ReceivedDate, request.Description, today);
  }

  public static TResult MapTo<TResult>(this Income income)
  {
    ArgumentNullException.ThrowIfNull(income);
    if (typeof(TResult) == typeof(UpdateIncomeDetailsResponse))
    {
      return (TResult)(object)new UpdateIncomeDetailsResponse(
        income.Id.Value,
        income.Source.Name,
        income.Amount.Value,
        income.Category.Name,
        income.ReceivedDate.Date,
        income.Description.Value,
        income.Version.Value);
    }

    return (TResult)(object)new AddIncomeResponse(
      income.Id.Value,
      income.Source.Name,
      income.Amount.Value,
      income.Category.Name,
      income.ReceivedDate.Date,
      income.Description.Value,
      income.Version.Value);
  }
}
