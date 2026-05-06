namespace Claims.Application.Abstractions.Services;

public interface IServiceBusService
{
    Task SendAsync(string auditOutbox, string messageId, CancellationToken cancellationToken);
}
