using CommandHub.Commands;
using FunctionalCore;

namespace ExpenseExplorer.ReadModel.Commands;

public sealed record CorrectStoreCommand(string ReceiptId, string Store) : ICommand<Unit>;
