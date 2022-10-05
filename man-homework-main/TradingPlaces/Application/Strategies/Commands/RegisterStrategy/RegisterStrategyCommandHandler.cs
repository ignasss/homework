using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Command;
using Models.Common;
using Models.Strategy;
using Persistence.Abstractions;

namespace Application.Strategies.Commands.RegisterStrategy
{
    public sealed class RegisterStrategyCommandHandler : ICommandHandler<RegisterStrategyCommand>
    {
        private const int MinTickerLength = 3;
        private const int MaxTickerLength = 5;
        private const decimal Zero = 0;
        private readonly IStrategiesRepository _strategiesRepository;

        public RegisterStrategyCommandHandler(IStrategiesRepository strategiesRepository)
        {
            _strategiesRepository = strategiesRepository;
        }

        public async Task<Result> Handle(RegisterStrategyCommand command, CancellationToken cancellationToken)
        {
            if (TickerNotValid(command.Ticker))
            {
                return new Result(
                    false,
                    new Error("Strategy.InvalidTicker", "Ticker identifier is an uppercase alphanumeric string of length 3 to 5 inclusive"));
            }

            if (command.PriceMovement == Zero)
            {
                return new Result(false, new Error("Strategy.InvalidPriceMovement", "Please specify price movement"));
            }

            if (command.Quantity <= Zero)
            {
                return new Result(false, new Error("Strategy.InvalidQuantity", "Quantity must be higher than 0"));
            }

            try
            {
                var strategyDetails = new StrategyDetails
                {
                    Ticker = command.Ticker,
                    Instruction = command.Instruction,
                    PriceMovement = command.PriceMovement,
                    Quantity = command.Quantity
                };
                await _strategiesRepository.Save(strategyDetails);
            }
            catch (ArgumentNullException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return new Result(true, Error.None);
        }

        private static bool TickerNotValid(string ticker)
        {
            if (ticker.Length < MinTickerLength || ticker.Length > MaxTickerLength)
            {
                return true;
            }

            if (ticker.Any(char.IsLower))
            {
                return true;
            }

            if (ticker.Any(char.IsSymbol))
            {
                return true;
            }

            return false;
        }
    }
}