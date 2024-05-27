namespace ExpenseExplorer.ReadModel.Models;

public record IncomeToExportReport(DateOnly StartDate, DateOnly EndDate, decimal TotalIncome, decimal TotalExpense);
