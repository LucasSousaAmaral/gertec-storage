using MediatR;
using Gertec.Storage.Domain.Entities;
using Gertec.Storage.Domain.Repositories;

namespace Gertec.Storage.Application.Commands;

public class AddStockCommandHandler : IRequestHandler<AddStockCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly IInventoryMovementRepository _movementRepository;

    public AddStockCommandHandler(
        IProductRepository productRepository,
        IInventoryMovementRepository movementRepository)
    {
        _productRepository = productRepository;
        _movementRepository = movementRepository;
    }

    public async Task<Unit> Handle(AddStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product is null)
            throw new Exception("Produto não encontrado para entrada de estoque.");

        var movement = new InventoryMovement
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            MovementDate = DateTime.UtcNow,
            Quantity = request.Quantity,
            MovementType = MovementType.In,
            UnitCost = product.AverageCost
        };

        await _movementRepository.AddAsync(movement);

        return Unit.Value;
    }
}
