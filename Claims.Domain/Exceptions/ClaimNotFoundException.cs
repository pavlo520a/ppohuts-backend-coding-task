namespace Claims.Domain.Exceptions;

public sealed class ClaimNotFoundException(string id) : NotFoundException("Claim", id);
