using Gertec.Storage.Domain.Entities;

namespace Gertec.Storage.Domain.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task<Product?> GetByIdAsync(Guid id);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
}