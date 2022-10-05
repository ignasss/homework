using System.Threading;
using System.Threading.Tasks;
using Application.Strategies.Commands.RegisterStrategy;
using Models.Common;
using Models.Strategy;
using Moq;
using NUnit.Framework;
using Persistence.Abstractions;

namespace ApplicationUnitTests
{
    public class RegisterStrategyTests
    {
        private RegisterStrategyCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            var strategiesRepositoryMock = new Mock<IStrategiesRepository>();
            _handler = new RegisterStrategyCommandHandler(strategiesRepositoryMock.Object);
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
            AssertFailure(result);
        }

        [Test]
        public async Task GivenStrategyDetails_WhenInvalidPriceMovement_ThenReturnsError()
        {
            //Arrange
            var command = new RegisterStrategyCommand("MSFT", Instruction.Buy, 0, 1);

            //Act
            var result = await _handler.Handle(command, new CancellationToken());

            //Assert
            AssertFailure(result);
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
            AssertFailure(result);
        }

        [Test]
        public async Task GivenStrategyDetails_WhenAllValuesValid_ThenReturnsSuccess()
        {
            //Arrange
            var command = new RegisterStrategyCommand("MSFT", Instruction.Buy, 1, 1);

            //Act
            var result = await _handler.Handle(command, new CancellationToken());

            //Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Error);
            Assert.IsEmpty(result.Error.Code);
            Assert.IsEmpty(result.Error.Message);
        }

        private static void AssertFailure(Result result)
        {
            Assert.IsTrue(result.IsFailure);
            Assert.IsNotNull(result.Error);
            Assert.IsNotEmpty(result.Error.Code);
            Assert.IsNotEmpty(result.Error.Message);
        }
    }
}