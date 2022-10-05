using System.Threading.Tasks;
using Persistence.Entities;

namespace Persistence.Abstractions
{
    public interface IDatabase
    {
        Task<Strategy[]> GetAll();
        Task<Strategy> Save(Strategy strategy);
        Task Remove(string id);
    }
}