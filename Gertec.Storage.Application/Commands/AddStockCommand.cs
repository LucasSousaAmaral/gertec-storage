using MediatR;

namespace Gertec.Storage.Application.Commands;

public record AddStockCommand(Guid ProductId, int Quantity) : IRequest<Unit>;
