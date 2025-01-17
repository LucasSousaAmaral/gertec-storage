using Gertec.Storage.Domain.Entities;

namespace Gertec.Storage.Domain.Repositories;

public interface IInventoryMovementRepository
{
    Task AddAsync(InventoryMovement movement);
    Task<IEnumerable<InventoryMovement>> GetMovementsByDateAsync(DateTime date, MovementType? type = null);
    Task<int> GetStockForProductAsync(Guid productId);
}