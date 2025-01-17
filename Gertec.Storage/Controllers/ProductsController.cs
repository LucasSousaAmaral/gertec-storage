using Microsoft.AspNetCore.Mvc;
using MediatR;
using Gertec.Storage.Application.Commands;
using Gertec.Storage.Application.Queries;

namespace Gertec.Storage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var productId = await _mediator.Send(command);
        return Ok(productId);
    }

    [HttpPost("{id:guid}/addstock")]
    public async Task<IActionResult> AddStock(Guid id, [FromBody] int quantity)
    {
        await _mediator.Send(new AddStockCommand(id, quantity));
        return NoContent();
    }

    [HttpPut("{id:guid}/consume")]
    public async Task<IActionResult> ConsumeProduct(Guid id, [FromBody] int quantity)
    {
        await _mediator.Send(new ConsumeProductCommand(id, quantity));
        return NoContent();
    }

    [HttpGet("consumption")]
    public async Task<IActionResult> GetDailyConsumption([FromQuery] DateTime date)
    {
        var result = await _mediator.Send(new GetDailyConsumptionQuery(date));
        return Ok(result);
    }
}
