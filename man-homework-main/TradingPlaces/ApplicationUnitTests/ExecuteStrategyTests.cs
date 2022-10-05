using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Strategies.Commands.ExecuteStrategy;
using Microsoft.Extensions.Logging;
using Models.Strategy;
using Moq;
using NUnit.Framework;
using Persistence.Abstractions;
using Persistence.Entities;
using Reutberg;

namespace ApplicationUnitTests
{
    public class ExecuteStrategyTests
    {
        private ExecuteStrategyCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            var strategiesRepositoryMock = new Mock<IStrategiesRepository>();

            var reutbergServiceMock = new Mock<IReutbergService>();
            reutbergServiceMock.Setup(r => r.GetQuote("MSFT")).Returns(() => 110);
            reutbergServiceMock.Setup(r => r.GetQuote("GOOGL")).Returns(() => 110);
            reutbergServiceMock.Setup(r => r.Buy("MSFT", 101)).Returns(() => 1);
            reutbergServiceMock.Setup(r => r.Sell("MSFT", 101)).Returns(() => -1);
            reutbergServiceMock.Setup(r => r.Buy("MSFT", 10)).Returns(() => 1100);
            reutbergServiceMock.Setup(r => r.Sell("MSFT", 10)).Returns(() => -1100);
            reutbergServiceMock.Setup(r => r.Buy("GOOGL", 100)).Throws(() => new TradeException("GOOGL"));
            reutbergServiceMock.Setup(r => r.Sell("GOOGL", 100)).Throws(() => new TradeException("GOOGL"));

            var loggerMock = new Mock<ILogger<ExecuteStrategyCommandHandler>>();

            _handler = new ExecuteStrategyCommandHandler(reutbergServiceMock.Object, strategiesRepositoryMock.Object, loggerMock.Object);
        }

        [Test]
        public async Task GivenStrategy_WhenPassedInvalidStrategy_ThenReturnsError()
        {
            //Arrange
            var command = new ExecuteStrategyCommand(null);

            //Act
            var result = await _handler.Handle(command, new CancellationToken());

            //Assert
            CommonAsserts.AssertFailure(result);
        }

        [Test]
        public async Task GivenStrategy_WhenExecutionCriteriaNotMet_ThenReturnsError()
        {
            //Arrange
            var command = new ExecuteStrategyCommand(new Strategy
            {
                Id = Guid.NewGuid().ToString(),
                Ticker = "MSFT",
                Instruction = Instruction.Buy,
                Quantity = 100,
                PriceMovement = 10,
                ExecutionPrice = 10000
            });

            //Act
            var result = await _handler.Handle(command, new CancellationToken());

            //Assert
            CommonAsserts.AssertFailure(result);
        }

        [Test]
        [TestCase(Instruction.Buy)]
        [TestCase(Instruction.Sell)]
        public async Task GivenStrategy_WhenExecutingAndExceptionOccurs_ThenReturnsError(Instruction instruction)
        {
            //Arrange
            var command = new ExecuteStrategyCommand(new Strategy
            {
                Id = Guid.NewGuid().ToString(),
                Ticker = "GOOGL",
                Instruction = instruction,
                Quantity = 100,
                PriceMovement = 10,
                ExecutionPrice = 110
            });

            //Act
            var result = await _handler.Handle(command, new CancellationToken());

            //Assert
            CommonAsserts.AssertFailure(result);
        }

        [Test]
        [TestCase(Instruction.Buy)]
        [TestCase(Instruction.Sell)]
        public async Task GivenStrategy_WhenExecutingAndReturnedInvalidResult_ThenReturnsError(Instruction instruction)
        {
            //Arrange
            var command = new ExecuteStrategyCommand(new Strategy
            {
                Id = Guid.NewGuid().ToString(),
                Ticker = "MSFT",
                Instruction = instruction,
                Quantity = 101,
                PriceMovement = 10,
                ExecutionPrice = 110
            });

            //Act
            var result = await _handler.Handle(command, new CancellationToken());

            //Assert
            CommonAsserts.AssertFailure(result);
        }

        [Test]
        [TestCase(Instruction.Buy)]
        [TestCase(Instruction.Sell)]
        public async Task GivenStrategy_WhenExecutingBuy_ThenReturnsSuccess(Instruction instruction)
        {
            //Arrange
            var command = new ExecuteStrategyCommand(new Strategy
            {
                Id = Guid.NewGuid().ToString(),
                Ticker = "MSFT",
                Instruction = instruction,
                Quantity = 10,
                PriceMovement = 10,
                ExecutionPrice = 11
            });

            //Act
            var result = await _handler.Handle(command, new CancellationToken());

            //Assert
            CommonAsserts.AssertSuccess(result);
        }
    }
}