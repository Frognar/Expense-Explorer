namespace ExpenseExplorer.ReadModel.Models.Persistence;

public sealed record DbPosition(ulong CommitPosition, ulong PreparePosition);
