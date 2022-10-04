using System.Threading.Tasks;
using Models.Strategy;

namespace Persistence.Abstractions
{
    public interface IStrategiesRepository
    {
        Task<Strategy[]> GetAll();
    }
}