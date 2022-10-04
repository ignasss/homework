﻿using System.Threading;
using System.Threading.Tasks;
using Application.Strategies.Commands.RegisterStrategy;
using Application.Strategies.Queries.GetAllStrategies;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly IMapper _mapper;

        public StrategyController(
            IHostedServiceAccessor<IStrategyManagementService> strategyManagementService,
            ILogger<StrategyController> logger,
            ISender sender, IMapper mapper)
        {
            _strategyManagementService = strategyManagementService;
            _logger = logger;
            _sender = sender;
            _mapper = mapper;
        }

        [HttpPost]
        [SwaggerOperation(nameof(RegisterStrategy))]
        [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(string))]
        public async Task<IActionResult> RegisterStrategy(StrategyDetailsDto strategyDetails, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new RegisterStrategyCommand(), cancellationToken);
            return result.IsSuccess ? (ActionResult)Ok() : BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(nameof(UnregisterStrategy))]
        [SwaggerResponse(StatusCodes.Status200OK, "OK")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found")]
        public async Task<IActionResult> UnregisterStrategy(string id, CancellationToken cancellationToken)
        {
            return Ok();
        }
    }
}
