using Claims.Application.Commands;
using Claims.Application.UseCases;
using Claims.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Claim>> GetAsync(
        [FromServices] IGetClaimsUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(new GetClaimsCommand(), cancellationToken);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(
        [FromBody] CreateClaimCommand command,
        [FromServices] ICreateClaimUseCase useCase,
        CancellationToken cancellationToken)
    {
        var claim = await useCase.ExecuteAsync(command, cancellationToken);
        return Ok(claim);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(
        string id,
        [FromServices] IDeleteClaimUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(new DeleteClaimCommand(id), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<Claim?> GetAsync(
        string id,
        [FromServices] IGetClaimByIdUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(new GetClaimByIdCommand(id), cancellationToken);
    }
}
