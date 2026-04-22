using Bluewave.Sales.Application.Interfaces;
using Bluewave.Sales.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Sales.Application.Features.Orders.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler(ISalesDbContext context)
    : IRequestHandler<GetAllOrdersQuery, List<Order>>
{
    public async Task<List<Order>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        return await context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }
}