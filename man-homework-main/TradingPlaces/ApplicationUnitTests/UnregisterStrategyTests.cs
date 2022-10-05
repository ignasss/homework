using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Strategies.Commands.UnregisterStrategy;
using Moq;
using NUnit.Framework;
using Persistence.Abstractions;

namespace ApplicationUnitTests
{
    public class UnregisterStrategyTests
    {
        private UnregisterStrategyCommandHandler _handler;
        private const string NotFoundErrorCode = "Strategy.NotFound";
        private string _notExistingStrategyId = "";
        private string _existingStrategyId = "";

        [SetUp]
        public void SetUp()
        {
            _notExistingStrategyId = Guid.NewGuid().ToString();
            _existingStrategyId = Guid.NewGuid().ToString();
            var strategiesRepositoryMock = new Mock<IStrategiesRepository>();
            strategiesRepositoryMock.Setup(r => r.Remove(_notExistingStrategyId)).Throws(new InvalidOperationException("Strategy.NotFound"));
            strategiesRepositoryMock.Setup(r => r.Remove(_existingStrategyId));
            _handler = new UnregisterStrategyCommandHandler(strategiesRepositoryMock.Object);
        }

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public async Task GivenId_WhenIdIsEmptyOrNull_ThenReturnsError(string id)
        {
            //Arrange
            var command = new UnregisterStrategyCommand(id);

            //Act
            var result = await _handler.Handle(command, new CancellationToken());

            //Assert
            CommonAsserts.AssertFailure(result);
        }

        [Test]
        public async Task GivenId_WhenStrategyNotFoundWithGivenId_ThenReturnsError()
        {
            //Arrange
            var command = new UnregisterStrategyCommand(_notExistingStrategyId);

            //Act
            var result = await _handler.Handle(command, new CancellationToken());

            //Assert
            CommonAsserts.AssertFailure(result);
            Assert.AreEqual(NotFoundErrorCode, result.Error.Code);
        }

        [Test]
        public async Task GivenId_WhenStrategyExists_ThenReturnsSuccess()
        {
            //Arrange
            var command = new UnregisterStrategyCommand(_existingStrategyId);

            //Act
            var result = await _handler.Handle(command, new CancellationToken());

            //Assert
            CommonAsserts.AssertSuccess(result);
        }
    }
}