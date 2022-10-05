using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistence.Abstractions;
using Persistence.Entities;

namespace Persistence.Database
{
    public class Database : IDatabase
    {
        private readonly ConcurrentDictionary<string, Strategy> _database;
        private readonly ILogger<Database> _logger;

        public Database(ILogger<Database> logger)
        {
            _logger = logger;
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
                _logger.LogInformation("Saved strategy with id: {newId} succssefully", newId);

                return await Task.FromResult(strategy);
            }

            _logger.LogInformation("Failed to save strategy");
            throw new InvalidOperationException("Couldn't save strategy to database");
        }

        public Task Remove(string id)
        {
            if (!_database.TryRemove(id, out var strategy))
            {
                _logger.LogInformation("Failed to remove strategy by id: {id}, strategy not found", id);
                throw new InvalidOperationException("Strategy.NotFound");
            }

            _logger.LogInformation("Strategy with id: {id}, removed", id);
            return Task.CompletedTask;
        }
    }
}