using Claims.Application.Commands;
using Claims.Application.UseCases;
using Claims.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    [HttpPost("compute")]
    public async Task<ActionResult> ComputePremiumAsync(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] CoverType coverType,
        [FromServices] IComputePremiumUseCase useCase,
        CancellationToken cancellationToken)
    {
        var command = new ComputePremiumCommand(startDate, endDate, coverType);
        var amount = await useCase.ExecuteAsync(command, cancellationToken);
        return Ok(amount);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync(
        [FromServices] IGetCoversUseCase useCase,
        CancellationToken cancellationToken)
    {
        var results = await useCase.ExecuteAsync(new GetCoversCommand(), cancellationToken);
        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cover?>> GetAsync(
        string id,
        [FromServices] IGetCoverByIdUseCase useCase,
        CancellationToken cancellationToken)
    {
        var cover = await useCase.ExecuteAsync(new GetCoverByIdCommand(id), cancellationToken);
        return Ok(cover);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(
        [FromBody] CreateCoverCommand command,
        [FromServices] ICreateCoverUseCase useCase,
        CancellationToken cancellationToken)
    {
        var cover = await useCase.ExecuteAsync(command, cancellationToken);
        return Ok(cover);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(
        string id,
        [FromServices] IDeleteCoverUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(new DeleteCoverCommand(id), cancellationToken);
    }
}
