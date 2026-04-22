using Bluewave.Sales.Domain.Entities;
using MediatR;

namespace Bluewave.Sales.Application.Features.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid Id) : IRequest<Order?>;