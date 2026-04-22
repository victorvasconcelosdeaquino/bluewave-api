using Bluewave.Inventory.Domain.Entities;
using MediatR;

namespace Bluewave.Inventory.Application.Features.Products.Queries.GetAllProducts;

public record GetAllProductsQuery(bool OnlyActive = true) : IRequest<List<Product>>;