using MediatR;
using Gertec.Storage.Domain.Entities;
using Gertec.Storage.Domain.Repositories;

namespace Gertec.Storage.Application.PipelineBehaviors;

public class ErrorLoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogRepository _logRepository;

    public ErrorLoggingBehavior(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var logEntry = new LogEntry
            {
                Id = Guid.NewGuid(),
                ErrorMessage = ex.Message,
                StackTrace = ex.StackTrace,
                CreatedDate = DateTime.UtcNow
            };

            await _logRepository.AddAsync(logEntry);

            throw;
        }
    }
}
