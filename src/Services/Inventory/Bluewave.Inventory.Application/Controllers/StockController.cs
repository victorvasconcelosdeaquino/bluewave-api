using Bluewave.Inventory.Application.Features.Stock.Commands.ReceiveStock;
using Bluewave.Inventory.Application.Features.Stock.Queries.GetStockOverview;
using Bluewave.Inventory.Application.Features.Stock.Queries.GetTransactionHistory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bluewave.Inventory.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class StockController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<StockOverviewDto>>> GetStockOverview()
    {
        var query = new GetStockOverviewQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("receive")]
    public async Task<IActionResult> ReceiveStock([FromBody] ReceiveStockCommand command)
    {
        var transactionId = await mediator.Send(command);

        return Ok(new { TransactionId = transactionId, Message = "Stock received successfully!" });
    }

    [HttpGet("history")]
    public async Task<ActionResult<PagedList<TransactionDto>>> GetHistory([FromQuery] GetTransactionHistoryQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }
}