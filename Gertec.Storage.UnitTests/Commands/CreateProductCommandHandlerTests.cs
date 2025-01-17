using Gertec.Storage.Application.Commands;
using Gertec.Storage.Domain.Entities;
using Gertec.Storage.Domain.Repositories;
using Moq;

namespace Gertec.Storage.UnitTests.Commands;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _handler = new CreateProductCommandHandler(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldCreateProductWithCorrectData()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Mouse Gamer XYZ",
            PartNumber: "MG-XYZ-123",
            AverageCost: 59.90m
        );

        // Act
        var productId = await _handler.Handle(command, default);

        // Assert
        Assert.NotEqual(Guid.Empty, productId);

        // Verifica se o repo foi chamado
        _productRepositoryMock.Verify(x => x.AddAsync(It.Is<Product>(p =>
            p.Name == "Mouse Gamer XYZ" &&
            p.PartNumber == "MG-XYZ-123" &&
            p.AverageCost == 59.90m
        )), Times.Once);
    }
}