using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Persistence.Abstractions;
using Persistence.Entities;

namespace Persistence.Database
{
    public class Database : IDatabase
    {
        private readonly ConcurrentDictionary<string, Strategy> _database;

        public Database()
        {
            _database = new ConcurrentDictionary<string, Strategy>();
        }

        public async Task<Strategy[]> GetAll()
        {
            return await Task.FromResult(_database.Select(d => d.Value).ToArray());
        }

        public async Task<Strategy> Save(Strategy strategy)
        {
            var newId = Guid.NewGuid().ToString();
            strategy.Id = newId;
            if (_database.TryAdd(newId, strategy))
            {
                return await Task.FromResult(strategy);
            }

            throw new InvalidOperationException("Couldn't save strategy to database");
        }

        public Task Remove(string id)
        {
            if (!_database.TryRemove(id, out var strategy))
            {
                throw new InvalidOperationException("Strategy.NotFound");
            }

            return Task.CompletedTask;
        }
    }
}