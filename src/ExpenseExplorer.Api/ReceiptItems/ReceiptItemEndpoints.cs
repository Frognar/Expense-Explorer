using ExpenseExplorer.Api.Http;
using ExpenseExplorer.Application.ReceiptItems;
using ExpenseExplorer.Contracts.ReceiptItems;
using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Api.ReceiptItems;

internal static class ReceiptItemEndpoints
{
    public static RouteGroupBuilder MapReceiptItems(this RouteGroupBuilder api)
    {
        api.MapGet("/receipt-items", ListAsync).WithTags("Receipt items");
        return api;
    }

    private static Task<IResult> ListAsync(
        [AsParameters] ReceiptItemListRequest request,
        IReceiptItemQueries queries,
        CancellationToken cancellationToken) =>
        ReceiptItemRequestParser.ParseList(request)
            .ToHttpAsync(
                async query => Result.Success(await queries.ListAsync(query, cancellationToken)),
                list => Results.Ok(list));
}
