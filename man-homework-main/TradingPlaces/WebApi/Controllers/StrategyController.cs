using System.Threading;
using System.Threading.Tasks;
using Application.Strategies.Commands.RegisterStrategy;
using Application.Strategies.Commands.UnregisterStrategy;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Strategy;
using Swashbuckle.AspNetCore.Annotations;
using TradingPlaces.WebApi.Dtos;
using TradingPlaces.WebApi.Services;

namespace TradingPlaces.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class StrategyController : ControllerBase
    {
        private readonly IHostedServiceAccessor<IStrategyManagementService> _strategyManagementService;
        private readonly ILogger<StrategyController> _logger;
        private readonly ISender _sender;
        private const string NotFoundErrorCode = "Strategy.NotFound";

        public StrategyController(
            IHostedServiceAccessor<IStrategyManagementService> strategyManagementService,
            ILogger<StrategyController> logger,
            ISender sender)
        {
            _strategyManagementService = strategyManagementService;
            _logger = logger;
            _sender = sender;
        }

        [HttpPost]
        [SwaggerOperation(nameof(RegisterStrategy))]
        [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request", typeof(string))]
        public async Task<IActionResult> RegisterStrategy(StrategyDetailsDto strategyDetails, CancellationToken cancellationToken)
        {
            var command = new RegisterStrategyCommand(
                strategyDetails.Ticker,
                (Instruction)strategyDetails.Instruction,
                strategyDetails.PriceMovement,
                strategyDetails.Quantity);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess ? (ActionResult)Ok() : BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(nameof(UnregisterStrategy))]
        [SwaggerResponse(StatusCodes.Status200OK, "OK")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request", typeof(string))]
        public async Task<IActionResult> UnregisterStrategy(string id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new UnregisterStrategyCommand(id), cancellationToken);
            if (result.IsFailure)
            {
                return result.Error.Code == NotFoundErrorCode ? (ActionResult)NotFound(id) : BadRequest(result.Error);
            }

            return Ok();
        }
    }
}
