using CommandHub.Commands;
using FunctionalCore;

namespace ExpenseExplorer.ReadModel.Commands;

public sealed record CorrectIncomeReceivedDateCommand(string IncomeId, DateOnly ReceivedDate) : ICommand<Unit>;
