using System;
using System.Threading.Tasks;
using Models.Strategy;
using Persistence.Abstractions;
using Persistence.Entities;

namespace Persistence.Repositories
{
    public class StrategiesRepository : IStrategiesRepository
    {
        private readonly IDatabase _database;

        public StrategiesRepository(IDatabase database)
        {
            _database = database;
        }

        public async Task<Strategy[]> GetAll()
        {
            return await _database.GetAll();
        }

        public async Task<Strategy> Save(StrategyDetails strategyDetails)
        {
            if (strategyDetails == null)
            {
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
                return result;
            }
            catch (InvalidOperationException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task Remove(string id)
        {
            try
            {
                await _database.Remove(id);
            }
            catch (InvalidOperationException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}