using AutoFixture;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.UseCase;
using Xunit;

namespace TenureInformationApi.Tests.V1.UseCase
{
    public class PostTenureUseCaseTests
    {
        private readonly Mock<ITenureGateway> _mockGateway;
        private readonly PostNewTenureUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        public PostTenureUseCaseTests()
        {
            _mockGateway = new Mock<ITenureGateway>();
            _classUnderTest = new PostNewTenureUseCase(_mockGateway.Object);
        }
        [Fact]
        public async Task CreateTenureByIdAsyncFoundReturnsResponse()
        {
            // Arrange
            var tenureRequest = new CreateTenureRequestObject();

            var tenure = _fixture.Create<TenureInformation>();

            _mockGateway.Setup(x => x.PostNewTenureAsync(tenureRequest)).ReturnsAsync(tenure);

            // Act
            var response = await _classUnderTest.ExecuteAsync(tenureRequest)
                .ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(tenure.ToResponse());
        }

        [Fact]
        public void CreateTenureByIdAsyncExceptionIsThrown()
        {
            // Arrange
            var tenureRequest = new CreateTenureRequestObject();
            var exception = new ApplicationException("Test exception");
            _mockGateway.Setup(x => x.PostNewTenureAsync(tenureRequest)).ThrowsAsync(exception);

            // Act
            Func<Task<TenureResponseObject>> func = async () => await _classUnderTest.ExecuteAsync(tenureRequest).ConfigureAwait(false);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }
    }
}
