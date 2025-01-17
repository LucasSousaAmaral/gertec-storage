using Dapper;
using Gertec.Storage.Domain.Entities;
using Gertec.Storage.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Gertec.Storage.IntegrationTests.Repositories;

public class InventoryMovementRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly IConfiguration _config;
    private readonly InventoryMovementRepository _movementRepo;
    private readonly string _connString;

    public InventoryMovementRepositoryIntegrationTests()
    {
        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json", optional: false)
            .Build();

        _connString = _config.GetConnectionString("MySqlConnection")
            ?? throw new Exception("ConnectionString not found.");

        _movementRepo = new InventoryMovementRepository(_config);
    }

    public async Task InitializeAsync()
    {
        using var conn = new MySqlConnection(_connString);
        await conn.OpenAsync();
        await conn.ExecuteAsync("DELETE FROM InventoryMovements;");
        await conn.ExecuteAsync("DELETE FROM Products;");
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task AddAsync_ShouldInsertMovement()
    {
        // AAA - Arrange
        var productId = Guid.NewGuid();
        using var conn = new MySqlConnection(_connString);
        await conn.ExecuteAsync(@"
            INSERT INTO Products (Id, Name, PartNumber, AverageCost)
            VALUES (@Id, @Name, @PartNumber, @AverageCost);
        ", new
        {
            Id = productId,
            Name = "Teclado",
            PartNumber = "TK-123",
            AverageCost = 150.00m
        });
        var movement = new InventoryMovement
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            MovementDate = DateTime.UtcNow,
            Quantity = 10,
            MovementType = MovementType.In,
            UnitCost = 150.0m
        };

        // AAA - Act
        await _movementRepo.AddAsync(movement);
        var total = await conn.ExecuteScalarAsync<int>(@"
            SELECT COALESCE(SUM(Quantity), 0)
            FROM InventoryMovements
            WHERE ProductId = @p;
        ", new { p = productId });

        // AAA - Assert
        Assert.Equal(10, total);
    }

    [Fact]
    public async Task GetStockForProductAsync_ShouldReturnSumOfMovements()
    {
        // AAA - Arrange
        var productId = Guid.NewGuid();
        using var conn = new MySqlConnection(_connString);
        await conn.ExecuteAsync(@"
            INSERT INTO Products (Id, Name, PartNumber, AverageCost)
            VALUES (@Id, @Name, @PartNumber, @AverageCost);
        ", new
        {
            Id = productId,
            Name = "Mouse",
            PartNumber = "MS-101",
            AverageCost = 60.0m
        });
        await conn.ExecuteAsync(@"
            INSERT INTO InventoryMovements (Id, ProductId, MovementDate, Quantity, MovementType, UnitCost)
            VALUES
            (@Id1, @P, @D, 10, 0, 60),
            (@Id2, @P, @D, -3, 1, 60);
        ", new
        {
            Id1 = Guid.NewGuid(),
            Id2 = Guid.NewGuid(),
            P = productId,
            D = DateTime.UtcNow
        });

        // AAA - Act
        var stock = await _movementRepo.GetStockForProductAsync(productId);

        // AAA - Assert
        Assert.Equal(7, stock);
    }

    [Fact]
    public async Task GetMovementsByDateAsync_ShouldReturnOnlySpecifiedDay()
    {
        // AAA - Arrange
        var productId = Guid.NewGuid();
        using var conn = new MySqlConnection(_connString);
        await conn.ExecuteAsync(@"
            INSERT INTO Products (Id, Name, PartNumber, AverageCost)
            VALUES (@Id, 'Monitor', 'MT-111', 800);
        ", new { Id = productId });
        var day1 = new DateTime(2025, 1, 17, 10, 0, 0, DateTimeKind.Utc);
        var day2 = new DateTime(2025, 1, 18, 10, 0, 0, DateTimeKind.Utc);
        await conn.ExecuteAsync(@"
            INSERT INTO InventoryMovements (Id, ProductId, MovementDate, Quantity, MovementType, UnitCost)
            VALUES
            (@Id1, @P, @D1, 5, 0, 800),
            (@Id2, @P, @D2, 10, 0, 800);
        ", new
        {
            Id1 = Guid.NewGuid(),
            Id2 = Guid.NewGuid(),
            P = productId,
            D1 = day1,
            D2 = day2
        });

        // AAA - Act
        var movementsDay1 = await _movementRepo.GetMovementsByDateAsync(day1, null);
        var movementsDay2 = await _movementRepo.GetMovementsByDateAsync(day2, null);

        // AAA - Assert
        Assert.Single(movementsDay1);
        Assert.Single(movementsDay2);
    }
}
