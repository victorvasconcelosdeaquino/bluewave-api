using Bluewave.Core.Messages.IntegrationEvents;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Bluewave.Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SimulationController(IPublishEndpoint publishEndpoint) : ControllerBase
{
    // POST api/simulation/sales
    [HttpPost("sales")]
    public async Task<IActionResult> SimulateSale([FromBody] OrderCreatedEvent request)
    {
        await publishEndpoint.Publish(request);

        return Ok(new { message = "Sales event published on RabbitMQ!" });
    }
}