using Gertec.Storage.Domain.Entities;
using Gertec.Storage.Domain.Repositories;
using MediatR;

namespace Gertec.Storage.Application.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            PartNumber = request.PartNumber,
            AverageCost = request.AverageCost
        };

        await _productRepository.AddAsync(product);

        return product.Id;
    }
}
