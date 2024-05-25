namespace ExpenseExplorer.API.Mappers;

using ExpenseExplorer.API.Contract;
using ExpenseExplorer.API.Contract.ReadModel;
using ExpenseExplorer.Application.Incomes.Commands;
using ExpenseExplorer.Domain.Incomes;

public static class IncomeMapper
{
  public static AddIncomeCommand MapToCommand(this AddIncomeRequest request, DateOnly today)
  {
    ArgumentNullException.ThrowIfNull(request);
    return new AddIncomeCommand(
      request.Source,
      request.Amount,
      request.Category,
      request.ReceivedDate,
      request.Description,
      today);
  }

  public static UpdateIncomeDetailsCommand MapToCommand(
    this UpdateIncomeDetailsRequest request,
    string incomeId,
    DateOnly today)
  {
    ArgumentNullException.ThrowIfNull(request);
    return new UpdateIncomeDetailsCommand(
      incomeId,
      request.Source,
      request.Amount,
      request.Category,
      request.ReceivedDate,
      request.Description,
      today);
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

  public static GetIncomesResponse MapToResponse(this ReadModel.Models.PageOf<ReadModel.Models.Income> page)
  {
    ArgumentNullException.ThrowIfNull(page);
    return new GetIncomesResponse(
      page.Items.Select(
        i => new IncomeResponse(
          i.Id,
          i.Source,
          i.Amount,
          i.Category,
          i.ReceivedDate,
          i.Description)),
      page.TotalCount,
      page.PageSize,
      page.PageNumber,
      page.PageCount);
  }

  public static GetIncomeResponse MapToResponse(this ReadModel.Models.Income income)
  {
    ArgumentNullException.ThrowIfNull(income);
    return new GetIncomeResponse(
      income.Id,
      income.Source,
      income.Amount,
      income.Category,
      income.ReceivedDate,
      income.Description);
  }
}
