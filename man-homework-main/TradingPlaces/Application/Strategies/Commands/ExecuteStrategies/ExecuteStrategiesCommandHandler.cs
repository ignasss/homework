using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Command;
using Application.Strategies.Commands.ExecuteStrategy;
using MediatR;
using Models.Common;
using Persistence.Abstractions;

namespace Application.Strategies.Commands.ExecuteStrategies
{
    public class ExecuteStrategiesCommandHandler : ICommandHandler<ExecuteStrategiesCommand>
    {
        private readonly IStrategiesRepository _strategiesRepository;
        private readonly ISender _sender;

        public ExecuteStrategiesCommandHandler(IStrategiesRepository strategiesRepository, ISender sender)
        {
            _strategiesRepository = strategiesRepository;
            _sender = sender;
        }

        public async Task<Result> Handle(ExecuteStrategiesCommand request, CancellationToken cancellationToken)
        {
            var strategies = await _strategiesRepository.GetAll();
            var loopResult = Parallel.ForEach(strategies, strategy =>
            {
                _sender.Send(new ExecuteStrategyCommand(strategy), cancellationToken);
            });

            return loopResult.IsCompleted
                ? new Result(true, Error.None)
                : new Result(false, new Error("Strategy.NotAllStrategiesExecuted", "Some strategies failed to execute"));
        }
    }
}