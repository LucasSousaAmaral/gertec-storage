using MediatR;

namespace Gertec.Storage.Application.Queries;

public record GetDailyConsumptionQuery(DateTime Date) : IRequest<DailyConsumptionDto>;

public record DailyConsumptionDto(
    DateTime Date,
    int TotalConsumed,
    decimal TotalCost
);