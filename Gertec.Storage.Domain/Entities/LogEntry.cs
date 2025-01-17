namespace Gertec.Storage.Domain.Entities;

public record LogEntry
{
    public Guid Id { get; init; }
    public string ErrorMessage { get; init; } = default!;
    public string? StackTrace { get; init; }
    public DateTime CreatedDate { get; init; }
}