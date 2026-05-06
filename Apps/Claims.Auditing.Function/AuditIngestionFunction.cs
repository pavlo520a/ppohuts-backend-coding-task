using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Claims.Auditing.Function;

public sealed class AuditIngestionFunction(
    IAuditWriteRepository auditWriteRepository,
    ILogger<AuditIngestionFunction> logger)
{
    [Function("AuditIngestionFunction")]
    public async Task RunAsync(
        [ServiceBusTrigger("%ServiceBus:QueueName%", Connection = "ServiceBus:ConnectionString")]
        string payload,
        CancellationToken cancellationToken)
    {
        var auditMessage = JsonSerializer.Deserialize<AuditMessage>(payload);
        if (auditMessage is null)
        {
            logger.LogWarning("Received empty or invalid audit payload.");
            return;
        }

        await auditWriteRepository.WriteAsync(auditMessage, cancellationToken);
    }
}
