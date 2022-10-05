using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Command;
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

        public ExecuteStrategyCommandHandler(IReutbergService reutbergService, IStrategiesRepository strategiesRepository)
        {
            _reutbergService = reutbergService;
            _strategiesRepository = strategiesRepository;
        }

        public async Task<Result> Handle(ExecuteStrategyCommand command, CancellationToken cancellationToken)
        {
            if (command.Strategy == null)
                return new Result(false, new Error("Strategy.Null", "Passed strategy value was null"));

            var currentPrice = _reutbergService.GetQuote(command.Strategy.Ticker);
            if (command.Strategy.PriceMovement > Zero && command.Strategy.ExecutionPrice <= currentPrice)
            {
                return await ExecuteStrategy(command.Strategy, currentPrice);
            }

            if (command.Strategy.PriceMovement < Zero && command.Strategy.ExecutionPrice >= currentPrice)
            {
                return await ExecuteStrategy(command.Strategy, currentPrice);
            }

            return new Result(false, new Error("Strategy.NotExecuted", $"The strategy with id: {command.Strategy.Id} was not executed"));
        }

        private async Task<Result> ExecuteStrategy(Strategy strategy, decimal currentPrice)
        {
            switch (strategy.Instruction)
            {
                case Instruction.Buy:
                {
                    try
                    {
                        var result = _reutbergService.Buy(strategy.Ticker, strategy.Quantity);
                        if (Math.Abs(result) == currentPrice * strategy.Quantity)
                        {
                            await _strategiesRepository.Remove(strategy.Id);
                            return new Result(true, Error.None);
                        }

                        return new Result(false, new Error("Strategy.InvalidExecution", $"During buy operation for strategy with id: {strategy.Id}, wrong amount was bought"));
                    }
                    catch (TradeException e)
                    {
                        return new Result(false, new Error("Strategy.NotExecuted", $"The strategy with id: {strategy.Id} failed to execute. TradeException: {e}"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
                case Instruction.Sell:
                {
                    try
                    {
                        var result = _reutbergService.Sell(strategy.Ticker, strategy.Quantity);
                        if (Math.Abs(result) == currentPrice * strategy.Quantity)
                        {
                            await _strategiesRepository.Remove(strategy.Id);
                            return new Result(true, Error.None);
                        }

                        return new Result(false, new Error("Strategy.InvalidExecution", $"During sell operation for strategy with id: {strategy.Id}, wrong amount was sold"));
                    }
                    catch (TradeException e)
                    {
                        return new Result(false, new Error("Strategy.NotExecuted", $"The strategy with id: {strategy.Id} failed to execute. TradeException: {e}"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
                default:
                    return new Result(false, new Error("Strategy.NotExecuted", $"The strategy with id: {strategy.Id} was not executed"));
            }
        }
    }
}