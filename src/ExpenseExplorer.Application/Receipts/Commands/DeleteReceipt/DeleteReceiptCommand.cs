namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using DotResult;
using FunctionalCore;

public record DeleteReceiptCommand(string ReceiptId) : ICommand<Result<Unit>>;
