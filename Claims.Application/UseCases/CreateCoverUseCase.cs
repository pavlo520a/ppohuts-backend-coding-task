using Claims.Application.Commands;
using Claims.Application.Premium;
using Claims.Data.Abstractions;
using Claims.Domain;

namespace Claims.Application.UseCases;

public sealed class CreateCoverUseCase(
    ICoverRepository coverRepository,
    IAuditTrailRepository auditTrailRepository,
    IPremiumCalculator premiumCalculator) : ICreateCoverUseCase
{
    public async Task<Cover> ExecuteAsync(CreateCoverCommand command, CancellationToken cancellationToken = default)
    {
        var cover = new Cover
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Type = command.Type,
            Premium = premiumCalculator.Compute(command.StartDate, command.EndDate, command.Type)
        };

        await coverRepository.AddAsync(cover, cancellationToken);
        await auditTrailRepository.WriteCoverAuditAsync(cover.Id, "POST", cancellationToken);
        return cover;
    }
}
