using MediatR;
using Models.Common;

namespace Application.Abstractions.Command
{
    public interface ICommand : IRequest<Result>
    {
    }

    public interface ICommand<TResult> : IRequest<Result<TResult>>
    {
    }
}