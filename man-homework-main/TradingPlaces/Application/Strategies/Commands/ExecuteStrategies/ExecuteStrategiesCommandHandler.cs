using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Command;
using Application.Strategies.Commands.ExecuteStrategy;
using MediatR;
using Microsoft.Extensions.Logging;
using Models.Common;
using Persistence.Abstractions;

namespace Application.Strategies.Commands.ExecuteStrategies
{
    public class ExecuteStrategiesCommandHandler : ICommandHandler<ExecuteStrategiesCommand>
    {
        private readonly IStrategiesRepository _strategiesRepository;
        private readonly ISender _sender;
        private readonly ILogger<ExecuteStrategiesCommandHandler> _logger;

        public ExecuteStrategiesCommandHandler(IStrategiesRepository strategiesRepository, ISender sender, ILogger<ExecuteStrategiesCommandHandler> logger)
        {
            _strategiesRepository = strategiesRepository;
            _sender = sender;
            _logger = logger;
        }

        public async Task<Result> Handle(ExecuteStrategiesCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting to process all strategies");

            var strategies = await _strategiesRepository.GetAll();

            _logger.LogInformation("Total strategies to process amount: {Amount}", strategies.Length);

            var loopResult = Parallel.ForEach(strategies, strategy =>
            {
                _sender.Send(new ExecuteStrategyCommand(strategy), cancellationToken);
            });

            if (loopResult.IsCompleted)
            {
                _logger.LogInformation("Finished to process all strategies");
                return new Result(true, Error.None);
            }

            _logger.LogInformation("Not all strategies processed succsesfully");
            return new Result(false, new Error("Strategy.NotAllStrategiesExecuted", "Some strategies failed to execute"));
        }
    }
}