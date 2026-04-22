using MediatR;

namespace Bluewave.Sales.Application.Features.Orders.Commands.CreateOrder;

public record OrderItemDto(Guid ProductId, string ProductName, decimal UnitPrice, decimal Quantity);

public record CreateOrderCommand(string CustomerName, List<OrderItemDto> Items) : IRequest<Guid>;