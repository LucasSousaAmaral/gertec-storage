using Gertec.Storage.Application.Commands;
using Gertec.Storage.Domain.Entities;
using Gertec.Storage.Domain.Repositories;
using Moq;

namespace Gertec.Storage.UnitTests.Commands;

public class AddStockCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IInventoryMovementRepository> _movementRepositoryMock;
    private readonly AddStockCommandHandler _handler;

    public AddStockCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _movementRepositoryMock = new Mock<IInventoryMovementRepository>();

        _handler = new AddStockCommandHandler(
            _productRepositoryMock.Object,
            _movementRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ThrowsException()
    {
        // Arrange
        var command = new AddStockCommand(Guid.NewGuid(), 10);

        // Retorna null simulando que não existe esse produto
        _productRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                              .ReturnsAsync((Product?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, default));
        Assert.Equal("Produto não encontrado para entrada de estoque.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldCreateMovementIn()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new AddStockCommand(productId, 20);

        var product = new Product
        {
            Id = productId,
            Name = "Teclado Mecânico",
            PartNumber = "TK-MEC-001",
            AverageCost = 150.50m
        };

        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId))
                              .ReturnsAsync(product);

        // Act
        await _handler.Handle(command, default);

        // Assert
        _movementRepositoryMock.Verify(x => x.AddAsync(It.Is<InventoryMovement>(m =>
            m.ProductId == productId &&
            m.Quantity == 20 &&
            m.MovementType == MovementType.In &&
            m.UnitCost == 150.50m
        )), Times.Once);
    }
}
