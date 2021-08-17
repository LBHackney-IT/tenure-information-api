using AutoFixture;
using FluentAssertions;
using Moq;
using System;
using System.Collections.ObjectModel;
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
    public class GetByIdUseCaseTests
    {
        private readonly Mock<ITenureGateway> _mockGateway;
        private readonly GetByIdUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IResponseFactory> _mockResponseFactory;
        public GetByIdUseCaseTests()
        {
            _mockGateway = new Mock<ITenureGateway>();

            _mockResponseFactory = new Mock<IResponseFactory>();
            _classUnderTest = new GetByIdUseCase(_mockGateway.Object, _mockResponseFactory.Object);
        }

        private GetByIdRequest ConstructRequest(Guid? id = null)
        {
            return new GetByIdRequest() { Id = id ?? Guid.NewGuid() };
        }

        [Fact]
        public async Task GetByIdUsecaseShouldBeNull()
        {
            var request = ConstructRequest();
            _mockGateway.Setup(x => x.GetEntityById(request)).ReturnsAsync((TenureInformation) null);

            var response = await _classUnderTest.Execute(request).ConfigureAwait(false);
            response.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdUsecaseShouldReturnOkResponse()
        {
            var tenure = _fixture.Create<TenureInformation>();
            var request = ConstructRequest(tenure.Id);
            _mockGateway.Setup(x => x.GetEntityById(request)).ReturnsAsync(tenure);


            var response = await _classUnderTest.Execute(request).ConfigureAwait(false);
            response.Should().BeEquivalentTo(_mockResponseFactory.Object.ToResponse(tenure));
        }

        [Fact]
        public void GetByIdThrowsException()
        {
            var request = ConstructRequest();
            var exception = new ApplicationException("Test Exception");
            _mockGateway.Setup(x => x.GetEntityById(request)).ThrowsAsync(exception);
            Func<Task<TenureResponseObject>> throwException = async () => await _classUnderTest.Execute(request).ConfigureAwait(false);
            throwException.Should().Throw<ApplicationException>().WithMessage("Test Exception");
        }
    }
}
