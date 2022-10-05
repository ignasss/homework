using System;
using System.Threading.Tasks;
using Application.Strategies.Commands.ExecuteStrategies;
using MediatR;
using Microsoft.Extensions.Logging;
using TradingPlaces.Resources;

namespace TradingPlaces.WebApi.Services
{
    internal class StrategyManagementService : TradingPlacesBackgroundServiceBase, IStrategyManagementService
    {
        private const int TickFrequencyMilliseconds = 1000;
        private readonly ISender _sender;

        public StrategyManagementService(ILogger<StrategyManagementService> logger, ISender sender)
            : base(TimeSpan.FromMilliseconds(TickFrequencyMilliseconds), logger)
        {
            _sender = sender;
        }

        protected override async Task CheckStrategies()
        {
            var command = new ExecuteStrategiesCommand();
            await _sender.Send(command);
        }
    }
}
