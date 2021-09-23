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
        private readonly DeletePersonFromTenureUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        public DeletePersonFromTenureUseCaseTests()
        {
            _mockGateway = new Mock<ITenureGateway>();

            _classUnderTest = new DeletePersonFromTenureUseCase(_mockGateway.Object);
        }

        [Fact]
        public async Task WhenCalledCallsUseCase()
        {
            // Arrange
            var mockQuery = _fixture.Create<DeletePersonFromTenureQueryRequest>();

            // Act
            await _classUnderTest.Execute(mockQuery).ConfigureAwait(false);

            // Assert
            _mockGateway.Verify(x => x.DeletePersonFromTenure(mockQuery));
        }
    }
}
