using Claims.ApiModels.Covers;
using Claims.Application.Commands.Covers;
using Claims.Domain.Models;

namespace Claims.Mapping;

public static class CoverApiMapping
{
    public static CreateCoverCommand ToCommand(this CreateCoverRequest request, string httpMethod) =>
        new()
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Type = request.Type,
            HttpMethod = httpMethod
        };

    public static CoverResponse ToResponse(this Cover cover) =>
        new()
        {
            Id = cover.Id,
            StartDate = cover.StartDate,
            EndDate = cover.EndDate,
            Type = cover.Type,
            Premium = cover.Premium
        };

    public static IReadOnlyList<CoverResponse> ToResponse(this IEnumerable<Cover> covers) =>
        [.. covers.Select(c => c.ToResponse())];
}
