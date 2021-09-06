using AutoFixture;
using FluentAssertions;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Moq;
using System;
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
    public class UpdateTenureForPersonUseCaseTests
    {
        private readonly Mock<ITenureGateway> _mockGateway;
        private readonly UpdateTenureForPersonUseCase _classUnderTest;
        private readonly Mock<ISnsGateway> _tenureSnsGateway;
        private readonly TenureSnsFactory _tenureSnsFactory;
        private readonly Fixture _fixture = new Fixture();

        public UpdateTenureForPersonUseCaseTests()
        {
            _mockGateway = new Mock<ITenureGateway>();
            _tenureSnsGateway = new Mock<ISnsGateway>();
            _tenureSnsFactory = new TenureSnsFactory();
            _classUnderTest = new UpdateTenureForPersonUseCase(_mockGateway.Object, _tenureSnsGateway.Object, _tenureSnsFactory);
        }

        private UpdateTenureRequest ConstructQuery(Guid? id = null, Guid? personId = null)
        {
            return new UpdateTenureRequest() { Id = id ?? Guid.NewGuid(), PersonId = personId ?? Guid.NewGuid() };
        }

        private UpdateTenureForPersonRequestObject ConstructUpdateRequest()
        {
            var request = _fixture.Create<UpdateTenureForPersonRequestObject>();

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
            var token = new Token();


            _mockGateway.Setup(x => x.UpdateTenureForPerson(query, request)).ReturnsAsync(updateTenure);
            var response = await _classUnderTest.ExecuteAsync(query, request, token).ConfigureAwait(false);
            response.Should().BeEquivalentTo(updateTenure.ToResponse());

        }

        [Fact]
        public async Task UpdateTenureByIdUseCaseReturnsNull()
        {
            var request = ConstructUpdateRequest();
            var query = ConstructQuery();
            var token = new Token();

            _mockGateway.Setup(x => x.UpdateTenureForPerson(query, request)).ReturnsAsync((TenureInformation) null);
            var response = await _classUnderTest.ExecuteAsync(query, request, token).ConfigureAwait(false);
            response.Should().BeNull();

        }

        [Fact]
        public void UpdateTenureByIdAsyncExceptionIsThrown()
        {
            // Arrange
            var request = ConstructUpdateRequest();
            var query = ConstructQuery();
            var token = new Token();
            var exception = new ApplicationException("Test exception");
            _mockGateway.Setup(x => x.UpdateTenureForPerson(query, request)).ThrowsAsync(exception);

            // Act
            Func<Task<TenureResponseObject>> func = async () =>
                await _classUnderTest.ExecuteAsync(query, request, token).ConfigureAwait(false);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }
    }
}
