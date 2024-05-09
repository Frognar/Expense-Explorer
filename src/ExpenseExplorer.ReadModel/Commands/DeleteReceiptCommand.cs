namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using FunctionalCore;

public sealed record DeleteReceiptCommand(string ReceiptId) : ICommand<Unit>;
