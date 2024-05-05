namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore;

public sealed record DeleteReceiptCommand(string ReceiptId) : ICommand<Unit>;
