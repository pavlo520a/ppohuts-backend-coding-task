namespace Claims.Application.Services;

public interface IServiceBusService
{
    Task SendAsync(string payload, string messageId, string subject, CancellationToken cancellationToken);
}
