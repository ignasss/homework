using System.Threading;
using System.Threading.Tasks;
using Application.Strategies.Commands.RegisterStrategy;
using Models.Strategy;
using Moq;
using NUnit.Framework;
using Persistence.Abstractions;
using Persistence.Entities;
using Reutberg;

namespace ApplicationUnitTests
{
    public class RegisterStrategyTests
    {
        private RegisterStrategyCommandHandler _handler;
        private const decimal Price = 100;

        [SetUp]
        public void Setup()
        {
            var strategiesRepositoryMock = new Mock<IStrategiesRepository>();
            strategiesRepositoryMock.Setup(r => r.Save(It.IsAny<StrategyDetails>())).ReturnsAsync((StrategyDetails s) => new Strategy
            {
                Ticker = s.Ticker,
                Instruction = s.Instruction,
                PriceMovement = s.PriceMovement,
                Quantity = s.Quantity,
                ExecutionPrice = s.ExecutionPrice
            });

            var reutbergServiceMock = new Mock<IReutbergService>();
            reutbergServiceMock.Setup(r => r.GetQuote("MSFT")).Returns(() => Price);
            reutbergServiceMock.Setup(r => r.GetQuote("GOOGL")).Throws(() => new QuoteException("GOOGL"));

            _handler = new RegisterStrategyCommandHandler(strategiesRepositoryMock.Object, reutbergServiceMock.Object);
        }

        [Test]
        [TestCase("aa")]
        [TestCase("aaaaaa")]
        [TestCase("aaa")]
        [TestCase("aaaa")]
        [TestCase("aaaaa")]
        [TestCase("aa*a")]
        public async Task GivenStrategyDetails_WhenInvalidTickerName_ThenReturnsError(string ticker)
        {
            //Arrange
            var command = new RegisterStrategyCommand(ticker, Instruction.Buy, 1, 1);

            //Act
            var result = await _handler.Handle(command, new CancellationToken());

            //Assert
            CommonAsserts.AssertFailure(result);
        }

        [Test]
        public async Task GivenStrategyDetails_WhenInvalidPriceMovement_ThenReturnsError()
        {
            //Arrange
            var command = new RegisterStrategyCommand("MSFT", Instruction.Buy, 0, 1);

            //Act
            var result = await _handler.Handle(command, new CancellationToken());

            //Assert
            CommonAsserts.AssertFailure(result);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public async Task GivenStrategyDetails_WhenInvalidQuantity_ThenReturnsError(int quantity)
        {
            //Arrange
            var command = new RegisterStrategyCommand("MSFT", Instruction.Buy, 1, quantity);

            //Act
            var result = await _handler.Handle(command, new CancellationToken());

            //Assert
            CommonAsserts.AssertFailure(result);
        }

        [Test]
        public async Task GivenStrategyDetails_WhenAllValuesValid_ThenReturnsSuccess()
        {
            //Arrange
            var command = new RegisterStrategyCommand("MSFT", Instruction.Buy, 1, 1);

            //Act
            var result = await _handler.Handle(command, new CancellationToken());

            //Assert
            CommonAsserts.AssertSuccess(result);
        }

        [Test]
        [TestCase(10, 110)]
        [TestCase(-10, 90)]
        public async Task GivenPriceMovement_WhenRegisteringStrategy_ThenExecutionPriceGetsCalculatedCorrectly(decimal priceMovement, decimal executionPrice)
        {
            //Arrange
            var command = new RegisterStrategyCommand("MSFT", Instruction.Buy, priceMovement, 10);

            //Act
            var result = await _handler.Handle(command, new CancellationToken());

            //Assert
            CommonAsserts.AssertSuccess(result);
            Assert.AreEqual(executionPrice, result.Value.ExecutionPrice);
        }

        [Test]
        public async Task GivenStrategy_WhenQueryingCurrentPriceFails_ThenReturnsError()
        {
            //Arrange
            var command = new RegisterStrategyCommand("GOOGL", Instruction.Buy, 10, 10);

            //Act
            var result = await _handler.Handle(command, new CancellationToken());

            //Assert
            CommonAsserts.AssertFailure(result);
        }
    }
}