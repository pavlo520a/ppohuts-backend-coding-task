using Claims.ApiModels;
using Claims.Application.Commands;
using Claims.Application.UseCases;
using Claims.Domain;
using Claims.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

/// <summary>
/// HTTP API for covers and premium calculation.
/// </summary>
[ApiController]
[Route("[controller]")]
[Tags("Covers")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class CoversController : ControllerBase
{
    /// <summary>
    /// Returns all covers.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CoverResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CoverResponse>>> GetAllAsync(
        [FromServices] IGetCoversUseCase useCase,
        CancellationToken cancellationToken)
    {
        var covers = await useCase.ExecuteAsync(new GetCoversCommand(), cancellationToken);
        var mapped = covers
            .Select(c => c.ToResponse())
            .ToList();

        return Ok(mapped);
    }

    /// <summary>
    /// Returns a single cover by identifier.
    /// </summary>
    /// <param name="id">Cover identifier.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CoverResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CoverResponse>> GetByIdAsync(
        [FromRoute] string id,
        [FromServices] IGetCoverByIdUseCase useCase,
        CancellationToken cancellationToken)
    {
        var cover = await useCase.ExecuteAsync(new GetCoverByIdCommand(id), cancellationToken);

        if (cover is null)
        {
            return NotFound();
        }

        return Ok(cover.ToResponse());
    }

    /// <summary>
    /// Creates a new cover with a server-calculated premium.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CoverResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CoverResponse>> CreateAsync(
        [FromBody] CreateCoverRequest request,
        [FromServices] ICreateCoverUseCase useCase,
        CancellationToken cancellationToken)
    {
        var cover = await useCase.ExecuteAsync(request.ToCommand(), cancellationToken);
        var response = cover.ToResponse();

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new
            {
                response.Id
            },
            response);
    }

    /// <summary>
    /// Computes premium for a date range and cover type (does not persist a cover).
    /// </summary>
    [HttpPost("compute")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    public async Task<ActionResult<decimal>> ComputePremiumAsync(
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

    /// <summary>
    /// Deletes a cover by identifier.
    /// </summary>
    /// <param name="id">Cover identifier.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] string id,
        [FromServices] IDeleteCoverUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(new DeleteCoverCommand(id), cancellationToken);
        return NoContent();
    }
}
