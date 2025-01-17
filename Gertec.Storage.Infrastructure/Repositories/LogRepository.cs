using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using Gertec.Storage.Domain.Entities;
using Gertec.Storage.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace Gertec.Storage.Infrastructure.Repositories;

public class LogRepository : ILogRepository
{
    private readonly string _connectionString;

    public LogRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySqlConnection")
            ?? throw new Exception("Connection string 'MySqlConnection' not found.");
    }

    public async Task AddAsync(LogEntry logEntry)
    {
        using IDbConnection db = new MySqlConnection(_connectionString);
        var sql = @"
            INSERT INTO Logs 
                (Id, ErrorMessage, StackTrace, CreatedDate)
            VALUES
                (@Id, @ErrorMessage, @StackTrace, @CreatedDate);
        ";
        await db.ExecuteAsync(sql, logEntry);
    }
}