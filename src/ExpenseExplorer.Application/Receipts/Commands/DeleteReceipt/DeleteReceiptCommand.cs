namespace ExpenseExplorer.Application.Receipts.Commands;

using CommandHub.Commands;
using FunctionalCore;
using FunctionalCore.Monads;

public record DeleteReceiptCommand(string ReceiptId) : ICommand<Result<Unit>>;
