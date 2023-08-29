using AutoFixture;
using FluentAssertions;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Hackney.Shared.Tenure.Boundary.Requests;
using Hackney.Shared.Tenure.Boundary.Response;
using Hackney.Shared.Tenure.Domain;
using Hackney.Shared.Tenure.Factories;
using Moq;
using System;
using System.Threading.Tasks;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Gateways.Interfaces;
using TenureInformationApi.V1.UseCase;
using Xunit;

namespace TenureInformationApi.Tests.V1.UseCase
{
    public class PostTenureUseCaseTests
    {
        private readonly Mock<ITenureDynamoDbGateway> _mockGateway;
        private readonly PostNewTenureUseCase _classUnderTest;
        private readonly Mock<ISnsGateway> _tenureSnsGateway;
        private readonly TenureSnsFactory _tenureSnsFactory;
        private readonly Fixture _fixture = new Fixture();

        public PostTenureUseCaseTests()
        {
            _mockGateway = new Mock<ITenureDynamoDbGateway>();
            _tenureSnsGateway = new Mock<ISnsGateway>();
            _tenureSnsFactory = new TenureSnsFactory();
            _classUnderTest = new PostNewTenureUseCase(_mockGateway.Object, _tenureSnsGateway.Object, _tenureSnsFactory);

        }
        [Fact]
        public async Task CreateTenureByIdAsyncFoundReturnsResponse()
        {
            // Arrange
            var tenureRequest = new CreateTenureRequestObject();
            var token = new Token();

            var tenure = _fixture.Create<TenureInformation>();

            _mockGateway.Setup(x => x.PostNewTenureAsync(tenureRequest)).ReturnsAsync(tenure);

            // Act
            var response = await _classUnderTest.ExecuteAsync(tenureRequest, token)
                .ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(tenure.ToResponse());
        }

        [Fact]
        public async Task CreateTenureByIdAsyncExceptionIsThrown()
        {
            // Arrange
            var tenureRequest = new CreateTenureRequestObject();
            var token = new Token();

            var exception = new ApplicationException("Test exception");
            _mockGateway.Setup(x => x.PostNewTenureAsync(tenureRequest)).ThrowsAsync(exception);

            // Act
            Func<Task<TenureResponseObject>> func = async () => await _classUnderTest.ExecuteAsync(tenureRequest, token).ConfigureAwait(false);

            // Assert
            (await func.Should().ThrowAsync<ApplicationException>()).WithMessage(exception.Message);
        }
    }
}
