using Claims.ApiModels.Covers;
using Claims.Application.Abstractions;
using Claims.Application.Commands.Covers;
using Claims.Domain.Models;
using Claims.Mapping;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Claims.Controllers;

/// <summary>
/// HTTP API for covers.
/// </summary>
[ApiController]
[Route("covers")]
[Tags("Covers")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[SwaggerResponse(StatusCodes.Status500InternalServerError, "An unexpected error occurred.", typeof(ProblemDetails))]
public class CoversController : ControllerBase
{
    /// <summary>
    /// Returns all covers.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CoverResponse>), StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns all covers.", typeof(IEnumerable<CoverResponse>))]
    public async Task<ActionResult<IEnumerable<CoverResponse>>> GetAllAsync(
        [FromServices] IUseCase<GetCoversCommand, IReadOnlyList<Cover>> useCase,
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
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the cover with the given id.", typeof(CoverResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No cover exists for the given id.")]
    public async Task<ActionResult<CoverResponse>> GetByIdAsync(
        [FromRoute] string id,
        [FromServices] IUseCase<GetCoverByIdCommand, Cover?> useCase,
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
    [SwaggerResponse(StatusCodes.Status201Created, "The cover was created with a server-calculated premium. The response body contains the new resource.", typeof(CoverResponse))]
    public async Task<ActionResult<CoverResponse>> CreateAsync(
        [FromBody] CreateCoverRequest request,
        [FromServices] IUseCase<CreateCoverCommand, Cover> useCase,
        CancellationToken cancellationToken)
    {
        var httpMethod = HttpContext.Request.Method.ToUpperInvariant();
        var cover = await useCase.ExecuteAsync(request.ToCommand(httpMethod), cancellationToken);
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
    /// Deletes a cover by identifier.
    /// </summary>
    /// <param name="id">Cover identifier.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status204NoContent, "The cover was deleted successfully.")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] string id,
        [FromServices] IUseCase<DeleteCoverCommand> useCase,
        CancellationToken cancellationToken)
    {
        var httpMethod = HttpContext.Request.Method.ToUpperInvariant();
        await useCase.ExecuteAsync(new DeleteCoverCommand(id, httpMethod), cancellationToken);
        return NoContent();
    }
}
