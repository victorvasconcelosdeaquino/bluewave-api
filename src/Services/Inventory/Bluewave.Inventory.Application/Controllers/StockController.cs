using Bluewave.Inventory.Application.Features.Stock.Queries.GetStockOverview;
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
}