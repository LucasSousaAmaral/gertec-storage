using Gertec.Storage.Domain.Entities;
using Gertec.Storage.Domain.Repositories;
using MediatR;

namespace Gertec.Storage.Application.Commands;

public class ConsumeProductCommandHandler : IRequestHandler<ConsumeProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly IInventoryMovementRepository _movementRepository;

    public ConsumeProductCommandHandler(
        IProductRepository productRepository,
        IInventoryMovementRepository movementRepository)
    {
        _productRepository = productRepository;
        _movementRepository = movementRepository;
    }

    public async Task<Unit> Handle(ConsumeProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product is null)
            throw new Exception("Peça inexistente.");

        var stock = await _movementRepository.GetStockForProductAsync(request.ProductId);
        if (stock < request.Quantity)
            throw new Exception("Saldo de estoque indisponível.");

        var movement = new InventoryMovement
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            MovementDate = DateTime.UtcNow,
            Quantity = -request.Quantity,
            MovementType = MovementType.Out,
            UnitCost = product.AverageCost
        };

        await _movementRepository.AddAsync(movement);

        return Unit.Value;
    }
}
