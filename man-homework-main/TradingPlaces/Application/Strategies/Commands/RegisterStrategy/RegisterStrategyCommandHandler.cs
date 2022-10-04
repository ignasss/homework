using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Command;
using Models.Common;

namespace Application.Strategies.Commands.RegisterStrategy
{
    internal sealed class RegisterStrategyCommandHandler : ICommandHandler<RegisterStrategyCommand>
    {
        public async Task<Result> Handle(RegisterStrategyCommand command, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}