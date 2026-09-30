using ExpenseExplorer.Contracts.Common;
using ExpenseExplorer.Contracts.Receipts;

namespace ExpenseExplorer.Application.Receipts;

/// <summary>Read side: answers straight from storage, bypassing the domain model.</summary>
public interface IReceiptQueries
{
    Task<ReceiptResponse?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<ReceiptListResponse> ListAsync(ReceiptListQuery query, CancellationToken cancellationToken);
}

/// <summary>Validated form of <see cref="ReceiptListRequest"/>: ranges are ordered, paging is within limits.</summary>
public sealed record ReceiptListQuery(
    IReadOnlyList<string> Stores,
    DateOnly? From,
    DateOnly? To,
    decimal? TotalMin,
    decimal? TotalMax,
    ReceiptSortField SortBy,
    SortDirection Direction,
    int Page,
    int PageSize);
