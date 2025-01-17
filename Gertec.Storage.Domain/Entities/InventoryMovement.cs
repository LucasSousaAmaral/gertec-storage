namespace Gertec.Storage.Domain.Entities;

public enum MovementType
{
    In = 0,
    Out = 1
}

public record InventoryMovement
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public DateTime MovementDate { get; init; }
    public int Quantity { get; init; }
    public MovementType MovementType { get; init; }
    public decimal UnitCost { get; init; }
}