using Claims.Application.Abstractions;
using Claims.Application.Abstractions.Formulas;
using Claims.Application.Commands.Covers;
using Claims.Data.Abstractions;
using Claims.Data.Abstractions.Repositories;
using Claims.Domain.Models;
using Claims.Domain.Models.Formulas;
using FluentValidation;

namespace Claims.Application.UseCases.Covers;

public sealed class CreateCoverUseCase(
    IUnitOfWork unitOfWork,
    IFormula<CoverPremiumFormulaArgs> premiumFormula,
    IValidator<CreateCoverCommand> validator) : IUseCase<CreateCoverCommand, Cover>
{
    public async Task<Cover> ExecuteAsync(CreateCoverCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var formulaArgs = new CoverPremiumFormulaArgs
        {
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            CoverType = command.Type
        };

        var cover = new Cover
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Type = command.Type,
            Premium = premiumFormula.Calculate(formulaArgs)
        };

        var auditOutbox = new AuditOutbox
        {
            EntityType = nameof(Cover),
            EntityId = cover.Id,
            HttpMethod = command.HttpMethod,
            OccurredAtUtc = DateTime.UtcNow
        };

        unitOfWork.CoversRepository.Add(cover);
        unitOfWork.OutboxRepository.Add(auditOutbox);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return cover;
    }
}
