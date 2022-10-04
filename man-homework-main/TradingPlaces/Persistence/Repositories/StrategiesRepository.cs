using System.Threading.Tasks;
using Models.Strategy;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class StrategiesRepository : IStrategiesRepository
    {
        public async Task<Strategy[]> GetAll()
        {
            return new Strategy[] { new Strategy
            {
                Instruction = Instruction.Sell,
                Quantity = 1,
                Ticker = "Test",
                PriceMovement = new decimal(10.0)
            }};
        }
    }
}