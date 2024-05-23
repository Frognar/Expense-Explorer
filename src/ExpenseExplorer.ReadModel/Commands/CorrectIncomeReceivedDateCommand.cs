namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using FunctionalCore;

public sealed record CorrectIncomeReceivedDateCommand(string IncomeId, DateOnly ReceivedDate) : ICommand<Unit>;
