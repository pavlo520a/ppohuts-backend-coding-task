namespace Claims.Domain.Exceptions;

public abstract class NotFoundException(string entityName, string entityId)
    : Exception($"{entityName} with id '{entityId}' was not found.")
{
    public string EntityName { get; } = entityName;

    public string EntityId { get; } = entityId;
}
