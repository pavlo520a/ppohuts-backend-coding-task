using Claims.Application.Commands.Claims;
using Claims.Application.Commands.Covers;
using Claims.Application.UseCases.Claims;
using Claims.Application.UseCases.Covers;
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
        var claimRepository = new StubClaimRepository();
        var useCase = new CreateClaimUseCase(claimRepository, new InlineValidator<CreateClaimCommand>());
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

        Assert.Equal(1, claimRepository.AddCalls);
    }

    [Fact]
    public async Task DeleteClaimUseCase_WritesToClaimRepository()
    {
        var claimRepository = new StubClaimRepository();
        var useCase = new DeleteClaimUseCase(claimRepository);

        await useCase.ExecuteAsync(
            new DeleteClaimCommand { Id = "claim-1", HttpMethod = "DELETE" },
            TestContext.Current.CancellationToken);

        Assert.Equal(1, claimRepository.DeleteCalls);
    }

    [Fact]
    public async Task CreateCoverUseCase_WritesToCoverRepository()
    {
        var coverRepository = new StubCoverRepository();
        var formula = new StubFormula();
        var useCase = new CreateCoverUseCase(coverRepository, formula, new InlineValidator<CreateCoverCommand>());
        var command = new CreateCoverCommand
        {
            StartDate = DateTime.UtcNow.Date.AddDays(1),
            EndDate = DateTime.UtcNow.Date.AddDays(10),
            Type = CoverType.Yacht,
            HttpMethod = "POST"
        };

        await useCase.ExecuteAsync(command, TestContext.Current.CancellationToken);

        Assert.Equal(1, coverRepository.AddCalls);
    }

    [Fact]
    public async Task DeleteCoverUseCase_WritesToCoverRepository()
    {
        var coverRepository = new StubCoverRepository();
        var useCase = new DeleteCoverUseCase(coverRepository);

        await useCase.ExecuteAsync(
            new DeleteCoverCommand { Id = "cover-1", HttpMethod = "DELETE" },
            TestContext.Current.CancellationToken);

        Assert.Equal(1, coverRepository.DeleteCalls);
    }

    private sealed class StubClaimRepository : IClaimRepository
    {
        public int AddCalls { get; private set; }
        public int DeleteCalls { get; private set; }

        public Task<IReadOnlyList<Claim>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Claim>>([]);
        }

        public Task<Claim?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult<Claim?>(null);
        }

        public Task AddAsync(Claim claim, string httpMethod, CancellationToken cancellationToken)
        {
            AddCalls++;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id, string httpMethod, CancellationToken cancellationToken)
        {
            DeleteCalls++;
            return Task.CompletedTask;
        }
    }

    private sealed class StubCoverRepository : ICoverRepository
    {
        public int AddCalls { get; private set; }
        public int DeleteCalls { get; private set; }

        public Task<IReadOnlyList<Cover>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Cover>>([]);
        }

        public Task<Cover?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult<Cover?>(null);
        }

        public Task AddAsync(Cover cover, string httpMethod, CancellationToken cancellationToken)
        {
            AddCalls++;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id, string httpMethod, CancellationToken cancellationToken)
        {
            DeleteCalls++;
            return Task.CompletedTask;
        }
    }

    private sealed class StubFormula : Claims.Application.Abstractions.Formulas.IFormula<CoverPremiumFormulaArgs>
    {
        public decimal Calculate(CoverPremiumFormulaArgs formulaArgs) => 123m;
    }
}
