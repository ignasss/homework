using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Command;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.Strategy;
using Persistence.Abstractions;
using Persistence.Entities;
using Reutberg;

namespace Application.Strategies.Commands.ExecuteStrategy
{
    public sealed class ExecuteStrategyCommandHandler : ICommandHandler<ExecuteStrategyCommand>
    {
        private const decimal Zero = 0;
        private readonly IReutbergService _reutbergService;
        private readonly IStrategiesRepository _strategiesRepository;
        private readonly ILogger<ExecuteStrategyCommandHandler> _logger;

        public ExecuteStrategyCommandHandler(IReutbergService reutbergService, IStrategiesRepository strategiesRepository, ILogger<ExecuteStrategyCommandHandler> logger)
        {
            _reutbergService = reutbergService;
            _strategiesRepository = strategiesRepository;
            _logger = logger;
        }

        public async Task<Result> Handle(ExecuteStrategyCommand command, CancellationToken cancellationToken)
        {
            if (command.Strategy == null)
            {
                _logger.LogInformation("Failed to execute strategy, strategy is null");
                return new Result(false, new Error("Strategy.Null", "Passed strategy value was null"));
            }

            var currentPrice = _reutbergService.GetQuote(command.Strategy.Ticker);
            if (command.Strategy.PriceMovement > Zero && command.Strategy.ExecutionPrice <= currentPrice)
            {
                return await ExecuteStrategy(command.Strategy, currentPrice);
            }

            if (command.Strategy.PriceMovement < Zero && command.Strategy.ExecutionPrice >= currentPrice)
            {
                return await ExecuteStrategy(command.Strategy, currentPrice);
            }

            _logger.LogInformation("Strategy with id: {Id} was not executed", command.Strategy.Id);
            return new Result(false, new Error("Strategy.NotExecuted", $"The strategy with id: {command.Strategy.Id} was not executed"));
        }

        private async Task<Result> ExecuteStrategy(Strategy strategy, decimal currentPrice)
        {
            decimal result = 0;
            try
            {
                switch (strategy.Instruction)
                {
                    case Instruction.Buy:
                        result = _reutbergService.Buy(strategy.Ticker, strategy.Quantity);
                        break;
                    case Instruction.Sell:
                        result = _reutbergService.Sell(strategy.Ticker, strategy.Quantity);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (TradeException e)
            {
                _logger.LogError(e, "Operation failed with exception");
                return new Result(false, new Error("Strategy.NotExecuted", $"The strategy with id: {strategy.Id} failed to execute. TradeException: {e}"));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Operation failed with exception");
                throw;
            }

            if (Math.Abs(result) == currentPrice * strategy.Quantity)
            {
                await _strategiesRepository.Remove(strategy.Id);
                _logger.LogInformation("Successfully executed strategy, id: {Id}", strategy.Id);
                return new Result(true, Error.None);
            }

            _logger.LogInformation("During operation wrong got back amount, strategy id: {id}, amount: {amount}", strategy.Id, result);
            return new Result(false, new Error("Strategy.InvalidExecution", $"During operation for strategy with id: {strategy.Id}, wrong amount was got back"));
        }
    }
}