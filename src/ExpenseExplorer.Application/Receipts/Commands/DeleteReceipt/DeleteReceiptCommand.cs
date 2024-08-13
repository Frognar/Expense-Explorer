using CommandHub.Commands;
using DotResult;
using FunctionalCore;

namespace ExpenseExplorer.Application.Receipts.Commands;

public record DeleteReceiptCommand(string ReceiptId) : ICommand<Result<Unit>>;
