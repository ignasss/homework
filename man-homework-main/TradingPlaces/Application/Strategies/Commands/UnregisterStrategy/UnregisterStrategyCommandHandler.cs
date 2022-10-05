using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Command;
using Models.Common;
using Persistence.Abstractions;

namespace Application.Strategies.Commands.UnregisterStrategy
{
    public sealed class UnregisterStrategyCommandHandler : ICommandHandler<UnregisterStrategyCommand>
    {
        private readonly IStrategiesRepository _strategiesRepository;

        public UnregisterStrategyCommandHandler(IStrategiesRepository strategiesRepository)
        {
            _strategiesRepository = strategiesRepository;
        }

        public async Task<Result> Handle(UnregisterStrategyCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(command.Id))
            {
                return new Result(false, new Error("Strategy.StrategyIdNotProvided", "Please provide strategy id"));
            }

            try
            {
                await _strategiesRepository.Remove(command.Id);
            }
            catch (InvalidOperationException)
            {
                return new Result(false, new Error("Strategy.NotFound", $"Strategy with provided id: {command.Id} not found"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return new Result(true, Error.None);
        }
    }
}