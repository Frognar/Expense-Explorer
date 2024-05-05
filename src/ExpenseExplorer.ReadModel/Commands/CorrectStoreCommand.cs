namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;
using FunctionalCore;

public sealed record CorrectStoreCommand(string ReceiptId, string Store) : ICommand<Unit>;
