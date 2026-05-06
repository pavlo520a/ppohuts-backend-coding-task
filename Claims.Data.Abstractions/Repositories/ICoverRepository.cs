using Claims.Domain.Models;

namespace Claims.Data.Abstractions.Repositories;

public interface ICoverRepository
{
    void Add(Cover cover);

    void Delete(string id);
}
