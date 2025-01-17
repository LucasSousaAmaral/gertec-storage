using Dapper;
using Gertec.Storage.Domain.Entities;
using Gertec.Storage.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Gertec.Storage.IntegrationTests.Repositories;

public class ProductRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly IConfiguration _config;
    private readonly ProductRepository _repository;
    private readonly string _connectionString;

    public ProductRepositoryIntegrationTests()
    {
        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json", optional: false)
            .Build();

        _connectionString = _config.GetConnectionString("MySqlConnection")
            ?? throw new Exception("Test ConnectionString not found.");

        _repository = new ProductRepository(_config);
    }

    public async Task InitializeAsync()
    {
        using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();
        await conn.ExecuteAsync("DELETE FROM InventoryMovements;");
        await conn.ExecuteAsync("DELETE FROM Products;");
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task AddAsync_ShouldInsertProduct()
    {
        // AAA - Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Mouse Gamer",
            PartNumber = "MG-001",
            AverageCost = 59.90m
        };

        // AAA - Act
        await _repository.AddAsync(product);
        var dbProduct = await _repository.GetByIdAsync(product.Id);

        // AAA - Assert
        Assert.NotNull(dbProduct);
        Assert.Equal("Mouse Gamer", dbProduct!.Name);
        Assert.Equal("MG-001", dbProduct.PartNumber);
        Assert.Equal(59.90m, dbProduct.AverageCost);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyProduct()
    {
        // AAA - Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Teclado",
            PartNumber = "TK-123",
            AverageCost = 150.00m
        };
        await _repository.AddAsync(product);
        var updated = product with { Name = "Teclado Mecânico", AverageCost = 200.0m };

        // AAA - Act
        await _repository.UpdateAsync(updated);
        var dbProduct = await _repository.GetByIdAsync(product.Id);

        // AAA - Assert
        Assert.NotNull(dbProduct);
        Assert.Equal("Teclado Mecânico", dbProduct!.Name);
        Assert.Equal(200.0m, dbProduct.AverageCost);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProduct()
    {
        // AAA - Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Monitor",
            PartNumber = "MT-001",
            AverageCost = 800.0m
        };
        await _repository.AddAsync(product);

        // AAA - Act
        await _repository.DeleteAsync(product.Id);
        var dbProduct = await _repository.GetByIdAsync(product.Id);

        // AAA - Assert
        Assert.Null(dbProduct);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductNotExists()
    {
        // AAA - Arrange
        var nonExistingId = Guid.NewGuid();

        // AAA - Act
        var dbProduct = await _repository.GetByIdAsync(nonExistingId);

        // AAA - Assert
        Assert.Null(dbProduct);
    }
}
