using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Claims.Application.Abstractions.Services;
using Claims.Application.HostedServices;
using Claims.Application.Options.Outbox;
using Claims.Data.Abstractions;
using Claims.Data.Abstractions.Queries;
using Claims.Data.Abstractions.Repositories;
using Claims.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Reflection;

namespace Claims.Application.UnitTests.HostedServices;

public class OutboxQueueHostedServiceTests
{
    private readonly Fixture _fixture = new();

    public OutboxQueueHostedServiceTests()
    {
        _fixture.Customize(new AutoNSubstituteCustomization
        {
            ConfigureMembers = true
        });
    }

    [Fact]
    public async Task ExecuteAsync_Should_MarkProcessingSendAndMarkSent()
    {
        // Arrange
        var messageId = "message-id-1";
        var payload = "{}";
        var attempts = 0;

        var serviceBusService = _fixture.Freeze<IServiceBusService>();
        var unitOfWork = _fixture.Freeze<IUnitOfWork>();
        var outboxRepository = _fixture.Freeze<IOutboxRepository>();
        var outboxQuery = _fixture.Freeze<IOutboxQuery>();

        unitOfWork.OutboxRepository
            .Returns(outboxRepository);

        var message = _fixture.Build<OutboxMessage>()
            .With(x => x.Id, messageId)
            .With(x => x.Payload, payload)
            .With(x => x.Attempts, attempts)
            .Create();

        IReadOnlyList<OutboxMessage> batch = [message];
        outboxQuery.GetPendingBatchAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(batch);

        var cancellationTokenSource = new CancellationTokenSource();
        serviceBusService
            .When(x => x.SendAsync(message.Payload, message.Id, Arg.Any<CancellationToken>()))
            .Do(_ => cancellationTokenSource.Cancel());

        serviceBusService
            .SendAsync(message.Payload, message.Id, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var sut = CreateSut(unitOfWork, outboxQuery, serviceBusService);

        // Act
        await ExecuteAsync(sut, cancellationTokenSource.Token);

        // Assert
        await outboxRepository
            .Received(1)
            .MarkProcessingAsync(message.Id, Arg.Any<CancellationToken>());

        await serviceBusService
            .Received(1)
            .SendAsync(message.Payload, message.Id, Arg.Any<CancellationToken>());

        await outboxRepository
            .Received(1)
            .MarkSentAsync(message.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_Should_RegisterAttempt_WhenSendFailsBelowMaxAttempts()
    {
        // Arrange
        var messageId = "message-id-1";
        var payload = "{}";
        var attempts = 0;

        var serviceBusService = _fixture.Freeze<IServiceBusService>();
        var unitOfWork = _fixture.Freeze<IUnitOfWork>();
        var outboxRepository = _fixture.Freeze<IOutboxRepository>();
        var outboxQuery = _fixture.Freeze<IOutboxQuery>();

        unitOfWork.OutboxRepository
            .Returns(outboxRepository);

        var message = _fixture.Build<OutboxMessage>()
            .With(x => x.Id, messageId)
            .With(x => x.Payload, payload)
            .With(x => x.Attempts, attempts)
            .Create();

        IReadOnlyList<OutboxMessage> batch = [message];
        outboxQuery.GetPendingBatchAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(batch);

        var cancellationTokenSource = new CancellationTokenSource();
        serviceBusService
            .SendAsync(message.Payload, message.Id, Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                cancellationTokenSource.Cancel();
                return Task.FromException(new InvalidOperationException("send failed"));
            });

        var sut = CreateSut(unitOfWork, outboxQuery, serviceBusService);

        // Act
        await ExecuteAsync(sut, cancellationTokenSource.Token);

        // Assert
        await outboxRepository
            .Received(1)
            .RegisterAttemptAsync(message.Id, Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_Should_MarkFailed_WhenMaxAttemptsReached()
    {
        // Arrange
        var messageId = "message-id-1";
        var payload = "{}";
        var attempts = 2;
        var maxAttempts = 3;

        var serviceBusService = _fixture.Freeze<IServiceBusService>();
        var unitOfWork = _fixture.Freeze<IUnitOfWork>();
        var outboxRepository = _fixture.Freeze<IOutboxRepository>();
        var outboxQuery = _fixture.Freeze<IOutboxQuery>();

        unitOfWork.OutboxRepository
            .Returns(outboxRepository);

        var message = _fixture.Build<OutboxMessage>()
            .With(x => x.Id, messageId)
            .With(x => x.Payload, payload)
            .With(x => x.Attempts, attempts)
            .Create();

        IReadOnlyList<OutboxMessage> batch = [message];
        outboxQuery.GetPendingBatchAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(batch);

        var cancellationTokenSource = new CancellationTokenSource();
        serviceBusService
            .SendAsync(message.Payload, message.Id, Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                cancellationTokenSource.Cancel();
                return Task.FromException(new InvalidOperationException("send failed"));
            });

        var sut = CreateSut(unitOfWork, outboxQuery, serviceBusService, maxAttempts: maxAttempts);

        // Act
        await ExecuteAsync(sut, cancellationTokenSource.Token);

        // Assert
        await outboxRepository
            .Received(1)
            .MarkFailedAsync(message.Id, Arg.Any<CancellationToken>());
    }

    private OutboxQueueHostedService CreateSut(
        IUnitOfWork unitOfWork,
        IOutboxQuery outboxQuery,
        IServiceBusService serviceBusService,
        int maxAttempts = 5)
    {
        var batchSize = 10;
        var pollIntervalMs = 1;

        var scopeFactory = _fixture.Freeze<IServiceScopeFactory>();
        var scope = _fixture.Freeze<IServiceScope>();
        var serviceProvider = _fixture.Freeze<IServiceProvider>();
        var logger = _fixture.Freeze<ILogger<OutboxQueueHostedService>>();

        var options = Microsoft.Extensions.Options.Options.Create(new OutboxProcessorOptions
        {
            BatchSize = batchSize,
            PollIntervalMs = pollIntervalMs,
            MaxAttempts = maxAttempts
        });

        scopeFactory.CreateScope()
            .Returns(scope);

        scope.ServiceProvider
            .Returns(serviceProvider);

        serviceProvider.GetService(typeof(IUnitOfWork))
            .Returns(unitOfWork);

        serviceProvider.GetService(typeof(IOutboxQuery))
            .Returns(outboxQuery);

        return new OutboxQueueHostedService(scopeFactory, options, serviceBusService, logger);
    }

    private static async Task ExecuteAsync(OutboxQueueHostedService sut, CancellationToken cancellationToken)
    {
        var executeAsyncMethod = typeof(OutboxQueueHostedService).GetMethod(
            "ExecuteAsync",
            BindingFlags.Instance | BindingFlags.NonPublic);

        if (executeAsyncMethod is null)
        {
            throw new InvalidOperationException("ExecuteAsync method was not found.");
        }

        var executionTask = (Task?)executeAsyncMethod.Invoke(sut, [cancellationToken]);

        if (executionTask is not null)
        {
            try
            {
                await executionTask;
            }
            catch (TaskCanceledException)
            {
                // Expected when the cancellation token is used to end the hosted loop.
            }
        }
    }
}
