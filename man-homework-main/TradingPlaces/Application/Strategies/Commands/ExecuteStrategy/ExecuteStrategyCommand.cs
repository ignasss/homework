using Application.Abstractions.Command;
using Persistence.Entities;

namespace Application.Strategies.Commands.ExecuteStrategy
{
    public sealed class ExecuteStrategyCommand : ICommand
    {
        public Strategy Strategy { get; }

        public ExecuteStrategyCommand(Strategy strategy)
        {
            Strategy = strategy;
        }
    }
}