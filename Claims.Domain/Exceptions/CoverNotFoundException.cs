namespace Claims.Domain.Exceptions;

public sealed class CoverNotFoundException(string id) : NotFoundException("Cover", id);
