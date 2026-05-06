using Claims.Domain.Models;

namespace Claims.Data.Abstractions.Repositories;

public interface IClaimRepository
{
    void Add(Claim claim);

    void Delete(string id);
}
