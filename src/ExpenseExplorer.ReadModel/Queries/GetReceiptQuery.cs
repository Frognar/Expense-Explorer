namespace ExpenseExplorer.ReadModel.Queries;

using CommandHub.Queries;
using ExpenseExplorer.ReadModel.Models;

public record GetReceiptQuery : IQuery<PageOf<ReceiptHeaders>>;
