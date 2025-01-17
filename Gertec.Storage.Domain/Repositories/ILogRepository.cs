using Gertec.Storage.Domain.Entities;

namespace Gertec.Storage.Domain.Repositories;

public interface ILogRepository
{
    Task AddAsync(LogEntry logEntry);
}