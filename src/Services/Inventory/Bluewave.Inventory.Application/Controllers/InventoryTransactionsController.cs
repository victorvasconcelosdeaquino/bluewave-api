using Bluewave.Inventory.Application.Features.InventoryTransactions.Commands.CreateTransaction;
using Bluewave.Inventory.Application.Features.InventoryTransactions.Queries.GetAllTransactions;
using Bluewave.Inventory.Application.Features.InventoryTransactions.Queries.GetTransactionById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bluewave.Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryTransactionsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? productId = null)
    {
        var result = await mediator.Send(new GetAllTransactionsQuery(productId));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetTransactionByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionCommand command)
    {
        try
        {
            var id = await mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message); // Returns 400 error if quantity is <= 0
        }
    }
}