using Claims.Data.Abstractions.Repositories;
using Claims.Data.Documents;
using Claims.Data.Mapping;
using Claims.Domain.Models;

namespace Claims.Data.Repositories;

public sealed class CoverRepository(ClaimsMongoDbContext context) : ICoverRepository
{
    public void Add(Cover cover)
    {
        var document = cover.ToDocument();
        context.Covers.Add(document);
    }

    public void Delete(string id)
    {
        context.Covers.Remove(new CoverDocument { Id = id });
    }
}
