using CommandHub.Commands;
using FunctionalCore;

namespace ExpenseExplorer.ReadModel.Commands;

public sealed record DeleteReceiptCommand(string ReceiptId) : ICommand<Unit>;
