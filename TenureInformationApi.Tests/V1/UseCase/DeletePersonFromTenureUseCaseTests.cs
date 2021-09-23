using AutoFixture;
using FluentAssertions;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.Tests.V1.Gateways;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Domain.Sns;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.Infrastructure;
using TenureInformationApi.V1.UseCase;
using Xunit;

namespace TenureInformationApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class DeletePersonFromTenureUseCaseTests
    {
        private readonly Mock<ITenureGateway> _mockGateway;
        private readonly Mock<ISnsGateway> _tenureSnsGateway;
        private readonly Mock<ISnsFactory> _tenureSnsFactory;

        private readonly DeletePersonFromTenureUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        public DeletePersonFromTenureUseCaseTests()
        {
            _mockGateway = new Mock<ITenureGateway>();
            _tenureSnsGateway = new Mock<ISnsGateway>();
            _tenureSnsFactory = new Mock<ISnsFactory>();

            _classUnderTest = new DeletePersonFromTenureUseCase(_mockGateway.Object, _tenureSnsGateway.Object, _tenureSnsFactory.Object);
        }

        [Fact]
        public async Task WhenCalledCallsUseCase()
        {
            // Arrange
            var mockQuery = _fixture.Create<DeletePersonFromTenureQueryRequest>();
            var mockToken = _fixture.Create<Token>();

            // Act
            await _classUnderTest.Execute(mockQuery, mockToken).ConfigureAwait(false);

            // Assert
            _mockGateway.Verify(x => x.DeletePersonFromTenure(mockQuery));
        }

        [Fact]
        public async Task WhenCalledPublishesEvent()
        {
            // Arrange
            var mockQuery = _fixture.Create<DeletePersonFromTenureQueryRequest>();
            var mockToken = _fixture.Create<Token>();
            var gatewayResult = _fixture.Create<UpdateEntityResult<TenureInformationDb>>();
            var snsEvent = _fixture.Create<TenureSns>();

            _mockGateway
                .Setup(x => x.DeletePersonFromTenure(It.IsAny<DeletePersonFromTenureQueryRequest>()))
                .ReturnsAsync(gatewayResult);

            _tenureSnsFactory
                 .Setup(x => x.PersonRemovedFromTenure(gatewayResult, It.IsAny<Token>()))
                 .Returns(snsEvent);

            // Act
            await _classUnderTest.Execute(mockQuery, mockToken).ConfigureAwait(false);

            // Assert
            _tenureSnsFactory.Verify(x => x.PersonRemovedFromTenure(gatewayResult, It.IsAny<Token>()), Times.Once);
            _tenureSnsGateway.Verify(x => x.Publish(snsEvent, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
