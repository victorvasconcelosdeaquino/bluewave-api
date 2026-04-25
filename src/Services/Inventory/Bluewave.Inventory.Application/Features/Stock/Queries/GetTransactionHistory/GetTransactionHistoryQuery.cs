using MediatR;

namespace Bluewave.Inventory.Application.Features.Stock.Queries.GetTransactionHistory;

public record TransactionDto(
    Guid Id,
    Guid ProductId,
    string TransactionType,
    decimal Quantity,
    DateTime CreatedAt,
    string? ReferenceDocument
);

public class PagedList<T>
{
    public List<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public bool HasNextPage => Page * PageSize < TotalCount;
    public bool HasPreviousPage => Page > 1;

    public PagedList(List<T> items, int count, int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = count;
        Items = items;
    }
}

public record GetTransactionHistoryQuery(
    Guid? ProductId,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedList<TransactionDto>>;