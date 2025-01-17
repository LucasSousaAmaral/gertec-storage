using Gertec.Storage.Application.Queries;
using Gertec.Storage.Domain.Entities;
using Gertec.Storage.Domain.Repositories;
using Moq;

namespace Gertec.Storage.UnitTests.Queries;

public class GetDailyConsumptionHandlerTests
{
    private readonly Mock<IInventoryMovementRepository> _movementRepositoryMock;
    private readonly GetDailyConsumptionHandler _handler;

    public GetDailyConsumptionHandlerTests()
    {
        _movementRepositoryMock = new Mock<IInventoryMovementRepository>();
        _handler = new GetDailyConsumptionHandler(_movementRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenNoMovements_ShouldReturnZeroConsumed()
    {
        // Arrange
        var date = new DateTime(2025, 1, 17);

        _movementRepositoryMock
            .Setup(x => x.GetMovementsByDateAsync(date, MovementType.Out))
            .ReturnsAsync(Array.Empty<InventoryMovement>());

        var query = new GetDailyConsumptionQuery(date);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        Assert.Equal(date, result.Date);
        Assert.Equal(0, result.TotalConsumed);
        Assert.Equal(0m, result.TotalCost);
    }

    [Fact]
    public async Task Handle_WhenHasOutMovements_ShouldSumQuantitiesAndCost()
    {
        // Arrange
        var date = new DateTime(2025, 1, 17);

        var movements = new[]
        {
            new InventoryMovement
            {
                Id = Guid.NewGuid(),
                MovementDate = date.AddHours(10),
                MovementType = MovementType.Out,
                Quantity = -3,
                UnitCost = 50.0m
            },
            new InventoryMovement
            {
                Id = Guid.NewGuid(),
                MovementDate = date.AddHours(11),
                MovementType = MovementType.Out,
                Quantity = -2,
                UnitCost = 60.0m
            }
        };

        _movementRepositoryMock
            .Setup(x => x.GetMovementsByDateAsync(date, MovementType.Out))
            .ReturnsAsync(movements);

        var query = new GetDailyConsumptionQuery(date);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        Assert.Equal(date, result.Date);

        // Movements = -3 e -2 => total 5
        Assert.Equal(5, result.TotalConsumed);

        // totalCost = (3 * 50) + (2 * 60) = 150 + 120 = 270
        Assert.Equal(270m, result.TotalCost);
    }
}
