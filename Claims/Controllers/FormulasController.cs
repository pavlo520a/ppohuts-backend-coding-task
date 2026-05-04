using Claims.ApiModels.Formulas;
using Claims.Application.Abstractions;
using Claims.Application.Commands.Formulas;
using Claims.Mapping;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Claims.Controllers;

/// <summary>
/// HTTP API for insurance formulas.
/// </summary>
[ApiController]
[Route("formulas")]
[Tags("Formulas")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[SwaggerResponse(StatusCodes.Status500InternalServerError, "An unexpected error occurred.", typeof(ProblemDetails))]
public class FormulasController : ControllerBase
{
    /// <summary>
    /// Computes premium for a date range and cover type.
    /// </summary>
    [HttpPost("cover/premium")]
    [ProducesResponseType(typeof(ComputePremiumResponse), StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the computed premium for the given date range and cover type.", typeof(ComputePremiumResponse))]
    public async Task<IActionResult> ComputePremiumAsync(
        [FromBody] ComputePremiumRequest request,
        [FromServices] IUseCase<ComputePremiumCommand, decimal> useCase,
        CancellationToken cancellationToken)
    {
        var httpMethod = HttpContext.Request.Method.ToUpperInvariant();
        var amount = await useCase.ExecuteAsync(request.ToCommand(httpMethod), cancellationToken);

        return Ok(new ComputePremiumResponse
        {
            Amount = amount
        });
    }
}
