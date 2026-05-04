using Claims.ApiModels;
using Claims.Application.Commands;
using Claims.Application.UseCases;
using Claims.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

/// <summary>
/// HTTP API for insurance claims.
/// </summary>
[ApiController]
[Route("[controller]")]
[Tags("Claims")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ClaimsController : ControllerBase
{
    /// <summary>
    /// Returns all claims.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClaimResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClaimResponse>>> GetAllAsync(
        [FromServices] IGetClaimsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var claims = await useCase.ExecuteAsync(new GetClaimsCommand(), cancellationToken);
        var mapped = claims
            .Select(c => c.ToResponse())
            .ToList();

        return Ok(mapped);
    }

    /// <summary>
    /// Returns a single claim by identifier.
    /// </summary>
    /// <param name="id">Claim identifier.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClaimResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClaimResponse>> GetByIdAsync(
        [FromRoute] string id,
        [FromServices] IGetClaimByIdUseCase useCase,
        CancellationToken cancellationToken)
    {
        var claim = await useCase.ExecuteAsync(new GetClaimByIdCommand(id), cancellationToken);

        if (claim is null)
        {
            return NotFound();
        }

        return Ok(claim.ToResponse());
    }

    /// <summary>
    /// Creates a new claim.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClaimResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<ClaimResponse>> CreateAsync(
        [FromBody] CreateClaimRequest request,
        [FromServices] ICreateClaimUseCase useCase,
        CancellationToken cancellationToken)
    {
        var claim = await useCase.ExecuteAsync(request.ToCommand(), cancellationToken);
        var response = claim.ToResponse();

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new
            {
                response.Id
            },
            response);
    }

    /// <summary>
    /// Deletes a claim by identifier.
    /// </summary>
    /// <param name="id">Claim identifier.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] string id,
        [FromServices] IDeleteClaimUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(new DeleteClaimCommand(id), cancellationToken);
        return NoContent();
    }
}
