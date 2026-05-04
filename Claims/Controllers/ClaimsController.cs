using Claims.ApiModels.Claims;
using Claims.Application.Abstractions;
using Claims.Application.Commands.Claims;
using Claims.Mapping;
using Claims.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Claims.Controllers;

/// <summary>
/// HTTP API for insurance claims.
/// </summary>
[ApiController]
[Route("claims")]
[Tags("Claims")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[SwaggerResponse(StatusCodes.Status500InternalServerError, "An unexpected error occurred.", typeof(ProblemDetails))]
public class ClaimsController : ControllerBase
{
    /// <summary>
    /// Returns all claims.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClaimResponse>), StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns all claims.", typeof(IReadOnlyList<ClaimResponse>))]
    public async Task<ActionResult<IReadOnlyList<ClaimResponse>>> GetAllAsync(
        [FromServices] IUseCase<GetClaimsCommand, IReadOnlyList<Claim>> useCase,
        CancellationToken cancellationToken)
    {
        var command = new GetClaimsCommand();
        var claims = await useCase.ExecuteAsync(command, cancellationToken);

        return Ok(claims.ToResponse());
    }

    /// <summary>
    /// Returns a single claim by identifier.
    /// </summary>
    /// <param name="id">Claim identifier.</param>
    [HttpGet("{id}")]
    [ActionName("GetByIdAsync")]
    [ProducesResponseType(typeof(ClaimResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the claim with the given id.", typeof(ClaimResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No claim exists for the given id.")]
    public async Task<ActionResult<ClaimResponse>> GetByIdAsync(
        [FromRoute] string id,
        [FromServices] IUseCase<GetClaimByIdCommand, Claim?> useCase,
        CancellationToken cancellationToken)
    {
        var command = new GetClaimByIdCommand
        {
            Id = id
        };

        var claim = await useCase.ExecuteAsync(command, cancellationToken);

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
    [SwaggerResponse(StatusCodes.Status201Created, "The claim was created. The response body contains the new resource.", typeof(ClaimResponse))]
    public async Task<ActionResult<ClaimResponse>> CreateAsync(
        [FromBody] CreateClaimRequest request,
        [FromServices] IUseCase<CreateClaimCommand, Claim> useCase,
        CancellationToken cancellationToken)
    {
        var httpMethod = HttpContext.Request.Method.ToUpperInvariant();
        var claim = await useCase.ExecuteAsync(
            request.ToCommand(httpMethod),
            cancellationToken);

        var response = claim.ToResponse();

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = response.Id },
            response);
    }

    /// <summary>
    /// Deletes a claim by identifier.
    /// </summary>
    /// <param name="id">Claim identifier.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status204NoContent, "The claim was deleted successfully.")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] string id,
        [FromServices] IUseCase<DeleteClaimCommand> useCase,
        CancellationToken cancellationToken)
    {
        var httpMethod = HttpContext.Request.Method.ToUpperInvariant();
        var command = new DeleteClaimCommand
        {
            Id = id,
            HttpMethod = httpMethod
        };

        await useCase.ExecuteAsync(command,cancellationToken);
        return NoContent();
    }
}
