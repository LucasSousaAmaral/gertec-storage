using MediatR;

namespace Gertec.Storage.Application.Commands;

public record CreateProductCommand(
    string Name,
    string PartNumber,
    decimal AverageCost
) : IRequest<Guid>;