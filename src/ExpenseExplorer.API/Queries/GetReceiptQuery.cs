namespace ExpenseExplorer.API.Queries;

using CommandHub.Queries;
using ExpenseExplorer.API.Contract;

public record GetReceiptQuery : IQuery<GetReceiptsResponse>;
