using Claims.Application.Commands.Claims;
using Claims.Application.Commands.Covers;
using Claims.Application.Options;
using Claims.Application.Validation.Claims;
using Claims.Application.Validation.Covers;
using Claims.Data.Abstractions.Repositories;
using Claims.Domain.Enums;
using Claims.Domain.Models;
using Microsoft.Extensions.Options;
using Xunit;

namespace Claims.Tests;

public sealed class ValidationTests
{
    [Fact]
    public async Task CreateCoverValidator_ReturnsError_WhenStartDateIsInPast()
    {
        var validator = new CreateCoverCommandValidator(CreateRulesOptions());
        var command = new CreateCoverCommand
        {
            StartDate = DateTime.UtcNow.Date.AddDays(-1),
            EndDate = DateTime.UtcNow.Date.AddDays(10),
            Type = CoverType.Yacht,
            HttpMethod = "POST"
        };

        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("StartDate cannot be in the past."));
    }

    [Fact]
    public async Task CreateCoverValidator_ReturnsError_WhenPeriodExceedsConfiguredLimit()
    {
        var validator = new CreateCoverCommandValidator(CreateRulesOptions());
        var command = new CreateCoverCommand
        {
            StartDate = DateTime.UtcNow.Date.AddDays(1),
            EndDate = DateTime.UtcNow.Date.AddDays(1).AddYears(1).AddDays(1),
            Type = CoverType.Yacht,
            HttpMethod = "POST"
        };

        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("cannot exceed 1 year"));
    }

    [Fact]
    public async Task CreateCoverValidator_AllowsEndDate_DayBeforeAnniversary()
    {
        var validator = new CreateCoverCommandValidator(CreateRulesOptions());
        var startDate = DateTime.UtcNow.AddDays(2);
        var command = new CreateCoverCommand
        {
            StartDate = startDate,
            EndDate = startDate.AddYears(1).AddDays(-1),
            Type = CoverType.Yacht,
            HttpMethod = "POST"
        };

        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task CreateCoverValidator_ReturnsError_WhenEndDateIsAnniversaryDay()
    {
        var validator = new CreateCoverCommandValidator(CreateRulesOptions());
        var startDate = DateTime.UtcNow.AddDays(2);
        var command = new CreateCoverCommand
        {
            StartDate = startDate,
            EndDate = startDate.AddYears(1),
            Type = CoverType.Yacht,
            HttpMethod = "POST"
        };

        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("cannot exceed 1 year"));
    }

    [Fact]
    public async Task CreateClaimValidator_ReturnsError_WhenDamageCostExceedsConfiguredLimit()
    {
        var coverRepository = new StubCoverRepository(CreateCover());
        var validator = new CreateClaimCommandValidator(coverRepository, CreateRulesOptions());
        var command = new CreateClaimCommand
        {
            CoverId = "cover-1",
            Created = DateTime.UtcNow.Date.AddDays(10),
            Name = "claim",
            Type = ClaimType.Fire,
            DamageCost = 100001m,
            HttpMethod = "POST"
        };

        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("DamageCost cannot exceed 100000"));
    }

    [Fact]
    public async Task CreateClaimValidator_ReturnsError_WhenCreatedDateIsOutsideCoverPeriod()
    {
        var coverRepository = new StubCoverRepository(CreateCover());
        var validator = new CreateClaimCommandValidator(coverRepository, CreateRulesOptions());
        var command = new CreateClaimCommand
        {
            CoverId = "cover-1",
            Created = DateTime.UtcNow.Date.AddDays(40),
            Name = "claim",
            Type = ClaimType.Collision,
            DamageCost = 1000m,
            HttpMethod = "POST"
        };

        var result = await validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Created date must be within the period"));
    }

    private static IOptions<ValidationRulesOptions> CreateRulesOptions()
    {
        return Options.Create(new ValidationRulesOptions
        {
            Claims = new ClaimValidationOptions
            {
                MaxDamageCost = 100000m
            },
            Covers = new CoverValidationOptions
            {
                MaxInsurancePeriodYears = 1
            }
        });
    }

    private static Cover CreateCover()
    {
        return new Cover
        {
            Id = "cover-1",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(30),
            Type = CoverType.Yacht,
            Premium = 1000m
        };
    }

    private sealed class StubCoverRepository(Cover? cover) : ICoverRepository
    {
        public Task<IReadOnlyList<Cover>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Cover>>([]);
        }

        public Task<Cover?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(cover?.Id == id ? cover : null);
        }

        public Task AddAsync(Cover cover, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task DeleteAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}
