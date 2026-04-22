using Bluewave.Sales.Domain.Entities;
using MediatR;

namespace Bluewave.Sales.Application.Features.Orders.Queries.GetAllOrders;

public record GetAllOrdersQuery() : IRequest<List<Order>>;