using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models.Strategy;
using Persistence.Abstractions;
using Persistence.Entities;

namespace Persistence.Repositories
{
    public class StrategiesRepository : IStrategiesRepository
    {
        private readonly IDatabase _database;
        private readonly ILogger<StrategiesRepository> _logger;

        public StrategiesRepository(IDatabase database, ILogger<StrategiesRepository> logger)
        {
            _database = database;
            _logger = logger;
        }

        public async Task<Strategy[]> GetAll()
        {
            return await _database.GetAll();
        }

        public async Task<Strategy> Save(StrategyDetails strategyDetails)
        {
            if (strategyDetails == null)
            {
                _logger.LogInformation("Failed to save strategy StrategyDetails is null");
                throw new ArgumentNullException(nameof(strategyDetails));
            }

            try
            {
                var strategy = new Strategy
                {
                    Ticker = strategyDetails.Ticker,
                    Instruction = strategyDetails.Instruction,
                    PriceMovement = strategyDetails.PriceMovement,
                    Quantity = strategyDetails.Quantity,
                    ExecutionPrice = strategyDetails.ExecutionPrice
                };
                var result = await _database.Save(strategy);
                _logger.LogInformation("Successfully saved strategy to database, id: {Id}", strategy.Id);
                return result;
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Failed to save strategy");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to save strategy");
                throw;
            }
        }

        public async Task Remove(string id)
        {
            try
            {
                await _database.Remove(id);
                _logger.LogInformation("Successfully removed strategy from database, id: {Id}", id);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Failed to remove strategy");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to remove strategy");
                throw;
            }
        }
    }
}