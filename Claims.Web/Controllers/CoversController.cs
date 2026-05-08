using Claims.Web.ApiModels.Covers;
using Claims.Application.Abstractions;
using Claims.Application.Commands.Covers;
using Claims.Domain.Models;
using Claims.Web.Mapping;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Claims.Web.Controllers;

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
    /// <param name="useCase">Use case that returns all covers.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CoverResponse>), StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns all covers.", typeof(IReadOnlyList<CoverResponse>))]
    public async Task<ActionResult<IReadOnlyList<CoverResponse>>> GetAllAsync(
        [FromServices] IUseCase<GetCoversCommand, IReadOnlyList<Cover>> useCase,
        CancellationToken cancellationToken)
    {
        var command = new GetCoversCommand();
        var covers = await useCase.ExecuteAsync(command, cancellationToken);

        return Ok(covers.ToResponse());
    }

    /// <summary>
    /// Returns a single cover by identifier.
    /// </summary>
    /// <param name="id">Cover identifier.</param>
    /// <param name="useCase">Use case that returns a cover by identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpGet("{id}")]
    [ActionName("GetByIdAsync")]
    [ProducesResponseType(typeof(CoverResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the cover with the given id.", typeof(CoverResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No cover exists for the given id.")]
    public async Task<ActionResult<CoverResponse>> GetByIdAsync(
        [FromRoute] string id,
        [FromServices] IUseCase<GetCoverByIdCommand, Cover> useCase,
        CancellationToken cancellationToken)
    {
        var command = new GetCoverByIdCommand
        {
            Id = id
        };

        var cover = await useCase.ExecuteAsync(command, cancellationToken);

        return Ok(cover.ToResponse());
    }

    /// <summary>
    /// Creates a new cover with a server-calculated premium.
    /// </summary>
    /// <param name="request">Cover payload used to create a new cover.</param>
    /// <param name="useCase">Use case that creates a cover.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpPost]
    [ProducesResponseType(typeof(CoverResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status201Created, "The cover was created with a server-calculated premium. The response body contains the new resource.", typeof(CoverResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed for the cover payload.", typeof(ValidationProblemDetails))]
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
            new { id = response.Id },
            response);
    }

    /// <summary>
    /// Deletes a cover by identifier.
    /// </summary>
    /// <param name="id">Cover identifier.</param>
    /// <param name="useCase">Use case that deletes a cover.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status204NoContent, "The cover was deleted successfully.")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] string id,
        [FromServices] IUseCase<DeleteCoverCommand> useCase,
        CancellationToken cancellationToken)
    {
        var httpMethod = HttpContext.Request.Method.ToUpperInvariant();
        var command = new DeleteCoverCommand
        {
            Id = id,
            HttpMethod = httpMethod
        };

        await useCase.ExecuteAsync(command, cancellationToken);
        return NoContent();
    }
}
