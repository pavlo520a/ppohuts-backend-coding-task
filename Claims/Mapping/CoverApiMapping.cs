using Claims.ApiModels.Covers;
using Claims.Application.Commands.Covers;
using Claims.Domain.Models;

namespace Claims.Mapping;

public static class CoverApiMapping
{
    public static CreateCoverCommand ToCommand(this CreateCoverRequest request) =>
        new(request.StartDate, request.EndDate, request.Type);

    public static CoverResponse ToResponse(this Cover cover) =>
        new()
        {
            Id = cover.Id,
            StartDate = cover.StartDate,
            EndDate = cover.EndDate,
            Type = cover.Type,
            Premium = cover.Premium
        };
}
