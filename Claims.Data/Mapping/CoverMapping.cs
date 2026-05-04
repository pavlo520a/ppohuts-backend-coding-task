using Claims.Data.Documents;
using Claims.Domain.Models;

namespace Claims.Data.Mapping;

public static class CoverMapping
{
    public static Cover ToDomain(this CoverDocument document) =>
        new()
        {
            Id = document.Id,
            StartDate = document.StartDate,
            EndDate = document.EndDate,
            Type = document.Type,
            Premium = document.Premium
        };

    public static CoverDocument ToDocument(this Cover cover) =>
        new()
        {
            Id = cover.Id,
            StartDate = cover.StartDate,
            EndDate = cover.EndDate,
            Type = cover.Type,
            Premium = cover.Premium
        };
}
