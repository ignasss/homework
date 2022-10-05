using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Command;
using Microsoft.Extensions.Logging;
using Models.Common;
using Persistence.Abstractions;

namespace Application.Strategies.Commands.UnregisterStrategy
{
    public sealed class UnregisterStrategyCommandHandler : ICommandHandler<UnregisterStrategyCommand>
    {
        private readonly IStrategiesRepository _strategiesRepository;
        private readonly ILogger<UnregisterStrategyCommandHandler> _logger;

        public UnregisterStrategyCommandHandler(IStrategiesRepository strategiesRepository, ILogger<UnregisterStrategyCommandHandler> logger)
        {
            _strategiesRepository = strategiesRepository;
            _logger = logger;
        }

        public async Task<Result> Handle(UnregisterStrategyCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(command.Id))
            {
                _logger.LogInformation("Strategy id not specified, id: {Id}", command.Id);
                return new Result(false, new Error("Strategy.StrategyIdNotProvided", "Please provide strategy id"));
            }

            try
            {
                await _strategiesRepository.Remove(command.Id);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Failed to remove strategy with id: {Id}", command.Id);
                return new Result(false, new Error("Strategy.NotFound", $"Strategy with provided id: {command.Id} not found"));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to remove strategy with id: {Id}", command.Id);
                throw;
            }

            _logger.LogInformation("Strategy removed succsessfully, id: {Id}", command.Id);
            return new Result(true, Error.None);
        }
    }
}