namespace ExpenseExplorer.ReadModel.Commands;

using CommandHub.Commands;

public sealed record CorrectStoreCommand(string ReceiptId, string Store) : ICommand<Unit>;
