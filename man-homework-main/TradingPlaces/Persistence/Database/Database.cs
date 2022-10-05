using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistence.Abstractions;
using Persistence.Entities;

namespace Persistence.Database
{
    public class Database : IDatabase
    {
        private readonly Dictionary<string, Strategy> _database;

        public Database(Dictionary<string, Strategy> database)
        {
            _database = database;
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
    }
}