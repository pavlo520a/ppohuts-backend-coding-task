using Claims.Application.Abstractions;
using Claims.Application.Abstractions.Formulas;
using Claims.Application.Commands.Covers;
using Claims.Data.Abstractions;
using Claims.Domain.Models;
using Claims.Domain.Models.Formulas;

namespace Claims.Application.UseCases.Covers;

public sealed class CreateCoverUseCase(
    ICoverRepository coverRepository,
    IAuditTrailRepository auditTrailRepository,
    IFormula<CoverPremiumFormulaArgs> premiumFormula) : IUseCase<CreateCoverCommand, Cover>
{
    public async Task<Cover> ExecuteAsync(CreateCoverCommand command, CancellationToken cancellationToken)
    {
        var formulaArgs = new CoverPremiumFormulaArgs(command.StartDate, command.EndDate, command.Type);
        var cover = new Cover
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Type = command.Type,
            Premium = premiumFormula.Calculate(formulaArgs)
        };

        await coverRepository.AddAsync(cover, cancellationToken);
        await auditTrailRepository.WriteCoverAuditAsync(cover.Id, "POST", cancellationToken);
        return cover;
    }
}
