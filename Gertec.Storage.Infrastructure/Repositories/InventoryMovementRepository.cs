using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using Gertec.Storage.Domain.Entities;
using Gertec.Storage.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace Gertec.Storage.Infrastructure.Repositories;

public class InventoryMovementRepository : IInventoryMovementRepository
{
    private readonly string _connectionString;

    public InventoryMovementRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySqlConnection")
            ?? throw new Exception("Connection string 'MySqlConnection' not found.");
    }

    public async Task AddAsync(InventoryMovement movement)
    {
        using IDbConnection db = new MySqlConnection(_connectionString);
        var sql = @"
            INSERT INTO InventoryMovements 
                (Id, ProductId, MovementDate, Quantity, MovementType, UnitCost)
            VALUES 
                (@Id, @ProductId, @MovementDate, @Quantity, @MovementType, @UnitCost);
        ";
        await db.ExecuteAsync(sql, movement);
    }

    public async Task<IEnumerable<InventoryMovement>> GetMovementsByDateAsync(DateTime date, MovementType? type = null)
    {
        using IDbConnection db = new MySqlConnection(_connectionString);
        var startDate = date.Date;
        var endDate = date.Date.AddDays(1).AddTicks(-1);

        var sql = @"
            SELECT * 
              FROM InventoryMovements
             WHERE MovementDate BETWEEN @StartDate AND @EndDate
        ";

        if (type.HasValue)
        {
            sql += " AND MovementType = @MovementType";
            return await db.QueryAsync<InventoryMovement>(sql,
                new { StartDate = startDate, EndDate = endDate, MovementType = type.Value });
        }
        else
        {
            return await db.QueryAsync<InventoryMovement>(sql,
                new { StartDate = startDate, EndDate = endDate });
        }
    }

    public async Task<int> GetStockForProductAsync(Guid productId)
    {
        using IDbConnection db = new MySqlConnection(_connectionString);
        var sql = @"
            SELECT COALESCE(SUM(Quantity), 0)
              FROM InventoryMovements
             WHERE ProductId = @ProductId;
        ";
        return await db.QuerySingleAsync<int>(sql, new { ProductId = productId });
    }
}
