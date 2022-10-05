﻿using System.Threading.Tasks;
using Persistence.Entities;

namespace Persistence.Abstractions
{
    public interface IDatabase
    {
        Task<Strategy> Save(Strategy strategy);
    }
}