using Claims.Application.Commands.Claims;
using Claims.Application.Commands.Covers;
using Claims.Application.UseCases.Claims;
using Claims.Application.UseCases.Covers;
using Claims.Data.Abstractions;
using Claims.Data.Abstractions.Repositories;
using Claims.Domain.Enums;
using Claims.Domain.Models;
using Claims.Domain.Models.Formulas;
using FluentValidation;

namespace Claims.Application.UnitTests.UseCases;

public sealed class OutboxUseCasesTests
{
    [Fact]
    public async Task CreateClaimUseCase_WritesToClaimRepository()
    {
        var unitOfWork = new StubUnitOfWork();
        var useCase = new CreateClaimUseCase(unitOfWork, new InlineValidator<CreateClaimCommand>());
        var command = new CreateClaimCommand
        {
            CoverId = "cover-1",
            Created = DateTime.UtcNow,
            Name = "claim-1",
            Type = ClaimType.Fire,
            DamageCost = 100m,
            HttpMethod = "POST"
        };

        await useCase.ExecuteAsync(command, TestContext.Current.CancellationToken);

        Assert.Equal(1, unitOfWork.ClaimsRepositoryStub.AddCalls);
        Assert.Equal(1, unitOfWork.OutboxRepositoryStub.AddCalls);
        Assert.Equal(1, unitOfWork.SaveCalls);
    }

    [Fact]
    public async Task DeleteClaimUseCase_WritesToClaimRepository()
    {
        var unitOfWork = new StubUnitOfWork();
        var useCase = new DeleteClaimUseCase(unitOfWork);

        await useCase.ExecuteAsync(
            new DeleteClaimCommand { Id = "claim-1", HttpMethod = "DELETE" },
            TestContext.Current.CancellationToken);

        Assert.Equal(1, unitOfWork.ClaimsRepositoryStub.DeleteCalls);
        Assert.Equal(1, unitOfWork.OutboxRepositoryStub.AddCalls);
        Assert.Equal(1, unitOfWork.SaveCalls);
    }

    [Fact]
    public async Task CreateCoverUseCase_WritesToCoverRepository()
    {
        var unitOfWork = new StubUnitOfWork();
        var formula = new StubFormula();
        var useCase = new CreateCoverUseCase(unitOfWork, formula, new InlineValidator<CreateCoverCommand>());
        var command = new CreateCoverCommand
        {
            StartDate = DateTime.UtcNow.Date.AddDays(1),
            EndDate = DateTime.UtcNow.Date.AddDays(10),
            Type = CoverType.Yacht,
            HttpMethod = "POST"
        };

        await useCase.ExecuteAsync(command, TestContext.Current.CancellationToken);

        Assert.Equal(1, unitOfWork.CoversRepositoryStub.AddCalls);
        Assert.Equal(1, unitOfWork.OutboxRepositoryStub.AddCalls);
        Assert.Equal(1, unitOfWork.SaveCalls);
    }

    [Fact]
    public async Task DeleteCoverUseCase_WritesToCoverRepository()
    {
        var unitOfWork = new StubUnitOfWork();
        var useCase = new DeleteCoverUseCase(unitOfWork);

        await useCase.ExecuteAsync(
            new DeleteCoverCommand { Id = "cover-1", HttpMethod = "DELETE" },
            TestContext.Current.CancellationToken);

        Assert.Equal(1, unitOfWork.CoversRepositoryStub.DeleteCalls);
        Assert.Equal(1, unitOfWork.OutboxRepositoryStub.AddCalls);
        Assert.Equal(1, unitOfWork.SaveCalls);
    }

    private sealed class StubClaimRepository : IClaimRepository
    {
        public int AddCalls { get; private set; }
        public int DeleteCalls { get; private set; }

        public void Add(Claim claim)
        {
            AddCalls++;
        }

        public void Delete(string id)
        {
            DeleteCalls++;
        }
    }

    private sealed class StubCoverRepository : ICoverRepository
    {
        public int AddCalls { get; private set; }
        public int DeleteCalls { get; private set; }

        public void Add(Cover cover)
        {
            AddCalls++;
        }

        public void Delete(string id)
        {
            DeleteCalls++;
        }
    }

    private sealed class StubOutboxRepository : IOutboxRepository
    {
        public int AddCalls { get; private set; }

        public void Add(BaseAuditOutbox auditOutbox)
        {
            AddCalls++;
        }

        public Task MarkProcessingAsync(string id, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task MarkSentAsync(string id, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task MarkFailedAsync(string id, string error, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class StubUnitOfWork : IUnitOfWork
    {
        public int SaveCalls { get; private set; }
        public StubClaimRepository ClaimsRepositoryStub { get; } = new();
        public StubCoverRepository CoversRepositoryStub { get; } = new();
        public StubOutboxRepository OutboxRepositoryStub { get; } = new();

        public IClaimRepository ClaimsRepository => ClaimsRepositoryStub;

        public ICoverRepository CoversRepository => CoversRepositoryStub;

        public IOutboxRepository OutboxRepository => OutboxRepositoryStub;

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveCalls++;
            return Task.CompletedTask;
        }
    }

    private sealed class StubFormula : Claims.Application.Abstractions.Formulas.IFormula<CoverPremiumFormulaArgs>
    {
        public decimal Calculate(CoverPremiumFormulaArgs formulaArgs) => 123m;
    }
}
