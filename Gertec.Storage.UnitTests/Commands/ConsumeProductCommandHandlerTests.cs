using Gertec.Storage.Application.Commands;
using Gertec.Storage.Domain.Entities;
using Gertec.Storage.Domain.Repositories;
using Moq;

namespace Gertec.Storage.UnitTests.Commands;

public class ConsumeProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IInventoryMovementRepository> _movementRepositoryMock;
    private readonly ConsumeProductCommandHandler _handler;

    public ConsumeProductCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _movementRepositoryMock = new Mock<IInventoryMovementRepository>();
        _handler = new ConsumeProductCommandHandler(
            _productRepositoryMock.Object,
            _movementRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ThrowsException()
    {
        // Arrange
        var command = new ConsumeProductCommand(Guid.NewGuid(), 5);

        // Simula que não encontrou o produto
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, default));
        Assert.Equal("Peça inexistente.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenStockIsNotEnough_ThrowsException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Teclado Mecânico",
            PartNumber = "TK-MEC-001",
            AverageCost = 150.50m
        };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _movementRepositoryMock
            .Setup(x => x.GetStockForProductAsync(productId))
            .ReturnsAsync(3);

        var command = new ConsumeProductCommand(productId, 5);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, default));
        Assert.Equal("Saldo de estoque indisponível.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenStockIsEnough_ShouldAddOutMovement()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Teclado Mecânico",
            PartNumber = "TK-MEC-001",
            AverageCost = 150.50m
        };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _movementRepositoryMock
            .Setup(x => x.GetStockForProductAsync(productId))
            .ReturnsAsync(10);

        var command = new ConsumeProductCommand(productId, 5);

        // Act
        await _handler.Handle(command, default);

        // Assert
        _movementRepositoryMock.Verify(x => x.AddAsync(It.Is<InventoryMovement>(m =>
            m.ProductId == productId &&
            m.Quantity == -5 &&
            m.MovementType == MovementType.Out &&
            m.UnitCost == 150.50m
        )), Times.Once);
    }
}
