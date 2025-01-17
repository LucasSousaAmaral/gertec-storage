using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using Gertec.Storage.Domain.Entities;
using Gertec.Storage.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace Gertec.Storage.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySqlConnection")
            ?? throw new Exception("Connection string 'MySqlConnection' not found.");
    }

    public async Task AddAsync(Product product)
    {
        using IDbConnection db = new MySqlConnection(_connectionString);
        var sql = @"
            INSERT INTO Products (Id, Name, PartNumber, AverageCost)
            VALUES (@Id, @Name, @PartNumber, @AverageCost);
        ";
        await db.ExecuteAsync(sql, product);
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        using IDbConnection db = new MySqlConnection(_connectionString);
        var sql = "SELECT * FROM Products WHERE Id = @Id;";
        return await db.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
    }

    public async Task UpdateAsync(Product product)
    {
        using IDbConnection db = new MySqlConnection(_connectionString);
        var sql = @"
            UPDATE Products 
               SET Name = @Name,
                   PartNumber = @PartNumber,
                   AverageCost = @AverageCost
             WHERE Id = @Id;
        ";
        await db.ExecuteAsync(sql, product);
    }

    public async Task DeleteAsync(Guid id)
    {
        using IDbConnection db = new MySqlConnection(_connectionString);
        var sql = "DELETE FROM Products WHERE Id = @Id;";
        await db.ExecuteAsync(sql, new { Id = id });
    }
}
