namespace Gertec.Storage.Domain.Entities;

public record Product
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string PartNumber { get; init; } = default!;
    public decimal AverageCost { get; init; }

    public Product WithNewAverageCost(decimal newCost)
        => this with { AverageCost = newCost };
}