using MediatR;
using Models.Common;

namespace Application.Abstractions.Query
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}