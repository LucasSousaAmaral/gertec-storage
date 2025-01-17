using MediatR;

namespace Gertec.Storage.Application.Commands;

public record ConsumeProductCommand(Guid ProductId, int Quantity) : IRequest<Unit>;