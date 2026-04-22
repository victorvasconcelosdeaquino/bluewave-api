using Bluewave.Inventory.Application.Features.Products.Commands.CreateProduct;
using Bluewave.Inventory.Application.Features.Products.Commands.UpdateProduct;
using Bluewave.Inventory.Application.Features.Products.Commands.DeactivateProduct;
using Bluewave.Inventory.Application.Features.Products.Queries.GetProductById;
using Bluewave.Inventory.Application.Features.Products.Queries.GetAllProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bluewave.Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = true)
    {
        var result = await mediator.Send(new GetAllProductsQuery(onlyActive));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch");

        var success = await mediator.Send(command);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var success = await mediator.Send(new DeactivateProductCommand(id));
        if (!success) return NotFound();

        return NoContent();
    }
}