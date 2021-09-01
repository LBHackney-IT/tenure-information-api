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
    [Collection("LogCall collection")]
    public class UpdateTenureUseCaseTests
    {
        private readonly Mock<ITenureGateway> _mockGateway;
        private readonly UpdateTenureUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        public UpdateTenureUseCaseTests()
        {
            _mockGateway = new Mock<ITenureGateway>();
            _classUnderTest = new UpdateTenureUseCase(_mockGateway.Object);
        }

        private TenureQueryRequest ConstructQuery(Guid? id = null, Guid? personId = null)
        {
            return new TenureQueryRequest() { Id = id ?? Guid.NewGuid(), PersonId = personId ?? Guid.NewGuid() };
        }

        private UpdateTenureRequestObject ConstructUpdateRequest()
        {
            var request = _fixture.Create<UpdateTenureRequestObject>();

            return request;
        }
        private TenureInformation ConstructTenure(Guid? id)
        {
            return _fixture.Build<TenureInformation>()
                           .With(x => x.Id, id)
                           .Create();
        }

        [Fact]
        public async Task UpdateTenureByIdUseCaseReturnsResult()
        {
            var request = ConstructUpdateRequest();
            var query = ConstructQuery();
            var updateTenure = ConstructTenure(query.Id);

            _mockGateway.Setup(x => x.UpdateTenure(query, request)).ReturnsAsync(updateTenure);
            var response = await _classUnderTest.ExecuteAsync(query, request).ConfigureAwait(false);
            response.Should().BeEquivalentTo(updateTenure.ToResponse());

        }

        [Fact]
        public async Task UpdateTenureByIdUseCaseReturnsNull()
        {
            var request = ConstructUpdateRequest();
            var query = ConstructQuery();

            _mockGateway.Setup(x => x.UpdateTenure(query, request)).ReturnsAsync((TenureInformation) null);
            var response = await _classUnderTest.ExecuteAsync(query, request).ConfigureAwait(false);
            response.Should().BeNull();

        }

        [Fact]
        public void UpdateTenureByIdAsyncExceptionIsThrown()
        {
            // Arrange
            var request = ConstructUpdateRequest();
            var query = ConstructQuery();
            var exception = new ApplicationException("Test exception");
            _mockGateway.Setup(x => x.UpdateTenure(query, request)).ThrowsAsync(exception);

            // Act
            Func<Task<TenureResponseObject>> func = async () =>
                await _classUnderTest.ExecuteAsync(query, request).ConfigureAwait(false);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }


    }
}
