using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.UseCase;
using Moq;
using System;
using TenureInformationApi.V1.Domain;
using FluentAssertions;
using AutoFixture;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Boundary.Response;
using Xunit;
using System.Threading.Tasks;

namespace TenureInformationApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class GetByIdUseCaseTests
    {
        private Mock<ITenureGateway> _mockGateway;
        private GetByIdUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();
        public GetByIdUseCaseTests()
        {
            _mockGateway = new Mock<ITenureGateway>();
            _classUnderTest = new GetByIdUseCase(_mockGateway.Object);
        }

        [Fact]
        public async Task GetByIdUsecaseShouldBeNull()
        {
            var id = Guid.NewGuid();
            _mockGateway.Setup(x => x.GetEntityById(id)).ReturnsAsync((TenureInformation) null);

            var response = await _classUnderTest.Execute(id).ConfigureAwait(false);
            response.Should().BeNull();
        }
        [Fact]
        public async Task GetByIdUsecaseShouldReturnOkResponse()
        {
            var tenure = _fixture.Create<TenureInformation>();
            _mockGateway.Setup(x => x.GetEntityById(tenure.Id)).ReturnsAsync(tenure);


            var response = await _classUnderTest.Execute(tenure.Id).ConfigureAwait(false);
            response.Should().BeEquivalentTo(tenure.ToResponse());
        }
        [Fact]
        public void GetByIdThrowsException()
        {
            var id = Guid.NewGuid();
            var exception = new ApplicationException("Test Exception");
            _mockGateway.Setup(x => x.GetEntityById(id)).ThrowsAsync(exception);
            Func<Task<TenureResponseObject>> throwException = async () => await _classUnderTest.Execute(id).ConfigureAwait(false);
            throwException.Should().Throw<ApplicationException>().WithMessage("Test Exception");

        }
    }
}
