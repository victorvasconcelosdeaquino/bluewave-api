using Bluewave.Sales.Application.Interfaces;
using Bluewave.Sales.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Sales.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler(ISalesDbContext context)
    : IRequestHandler<GetOrderByIdQuery, Order?>
{
    public async Task<Order?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        return await context.Orders
            .AsNoTracking()
            .Include(o => o.Items) // Include order items
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
    }
}