using Gertec.Storage.Domain.Entities;
using Gertec.Storage.Domain.Repositories;
using MediatR;

namespace Gertec.Storage.Application.Queries;

public class GetDailyConsumptionHandler
    : IRequestHandler<GetDailyConsumptionQuery, DailyConsumptionDto>
{
    private readonly IInventoryMovementRepository _movementRepository;

    public GetDailyConsumptionHandler(IInventoryMovementRepository movementRepository)
    {
        _movementRepository = movementRepository;
    }

    public async Task<DailyConsumptionDto> Handle(GetDailyConsumptionQuery request, CancellationToken cancellationToken)
    {
        var movements = await _movementRepository.GetMovementsByDateAsync(request.Date, MovementType.Out);

        int totalConsumed = 0;
        decimal totalCost = 0;

        foreach (var mv in movements)
        {
            int qty = mv.Quantity < 0 ? mv.Quantity * -1 : mv.Quantity;
            totalConsumed += qty;
            totalCost += qty * mv.UnitCost;
        }

        return new DailyConsumptionDto(
            request.Date,
            totalConsumed,
            totalCost
        );
    }
}
