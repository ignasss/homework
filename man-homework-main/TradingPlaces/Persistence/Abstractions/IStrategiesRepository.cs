using System.Threading.Tasks;
using Models.Strategy;
using Persistence.Entities;

namespace Persistence.Abstractions
{
    public interface IStrategiesRepository
    {
        Task<Strategy[]> GetAll();
        Task<Strategy> Save(StrategyDetails strategyDetails);
        Task Remove(string id);
    }
}