using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Command;
using Models.Common;
using Models.Strategy;
using Persistence.Abstractions;
using Reutberg;

namespace Application.Strategies.Commands.RegisterStrategy
{
    public sealed class RegisterStrategyCommandHandler : ICommandHandler<RegisterStrategyCommand, StrategyDetails>
    {
        private const int MinTickerLength = 3;
        private const int MaxTickerLength = 5;
        private const decimal Zero = 0;
        private readonly IStrategiesRepository _strategiesRepository;
        private readonly IReutbergService _reutbergService;

        public RegisterStrategyCommandHandler(IStrategiesRepository strategiesRepository, IReutbergService reutbergService)
        {
            _strategiesRepository = strategiesRepository;
            _reutbergService = reutbergService;
        }

        public async Task<Result<StrategyDetails>> Handle(RegisterStrategyCommand command, CancellationToken cancellationToken)
        {
            if (TickerNotValid(command.Ticker))
            {
                return new Result<StrategyDetails>(
                    default,
                    false,
                    new Error("Strategy.InvalidTicker", "Ticker identifier is an uppercase alphanumeric string of length 3 to 5 inclusive"));
            }

            if (command.PriceMovement == Zero)
            {
                return new Result<StrategyDetails>(default, false, new Error("Strategy.InvalidPriceMovement", "Please specify price movement"));
            }

            if (command.Quantity <= Zero)
            {
                return new Result<StrategyDetails>(default, false, new Error("Strategy.InvalidQuantity", "Quantity must be higher than 0"));
            }

            var strategyDetails = new StrategyDetails
            {
                Ticker = command.Ticker,
                Instruction = command.Instruction,
                PriceMovement = command.PriceMovement,
                Quantity = command.Quantity
            };
            decimal currentPrice;
            try
            {
                currentPrice = _reutbergService.GetQuote(command.Ticker);
            }
            catch (QuoteException e)
            {
                return new Result<StrategyDetails>(default, false, new Error("Strategy.QuoteQueryFailed", $"Failed to retrieve quote, QuoteException: {e}"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var priceDifference = Math.Abs(command.PriceMovement) / 100 * currentPrice;
            if (command.PriceMovement < 0)
            {
                strategyDetails.ExecutionPrice = currentPrice - priceDifference;
            }
            else
            {
                strategyDetails.ExecutionPrice = currentPrice + priceDifference;
            }

            try
            {
                var strategy = await _strategiesRepository.Save(strategyDetails);
                var savedStrategyDetails = new StrategyDetails
                {
                    Ticker = strategy.Ticker,
                    Instruction = strategy.Instruction,
                    PriceMovement = strategy.PriceMovement,
                    Quantity = strategy.Quantity,
                    ExecutionPrice = strategy.ExecutionPrice
                };
                return new Result<StrategyDetails>(savedStrategyDetails, true, Error.None);
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