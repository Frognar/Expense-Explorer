namespace ExpenseExplorer.API.Contract.ReadModel;

public record GenerateIncomeToExpenseReportResponse(
  DateOnly StartDate,
  DateOnly EndDate,
  decimal TotalIncome,
  decimal TotalExpense);
