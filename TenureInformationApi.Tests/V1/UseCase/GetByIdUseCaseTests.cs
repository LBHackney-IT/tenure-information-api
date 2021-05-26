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

namespace TenureInformationApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class GetByIdUseCaseTests
    {
        private Mock<IExampleGateway> _mockGateway;
        private GetByIdUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();
        public GetByIdUseCaseTests()
        {
            _mockGateway = new Mock<IExampleGateway>();
            _classUnderTest = new GetByIdUseCase(_mockGateway.Object);
        }

        [Fact]
        public void GetByIdUsecaseShouldBeNull()
        {
            var id = Guid.NewGuid();
            _mockGateway.Setup(x => x.GetEntityById(id)).Returns((TenureInformation) null);

            var response = _classUnderTest.Execute(id);
            response.Should().BeNull();
        }
        [Fact]
        public void GetByIdUsecaseShouldReturnOkResponse()
        {
            var tenure = _fixture.Create<TenureInformation>();
            _mockGateway.Setup(x => x.GetEntityById(tenure.Id)).Returns(tenure);


            var response = _classUnderTest.Execute(tenure.Id);
            response.Should().BeEquivalentTo(tenure.ToResponse());
        }
        [Fact]
        public void GetByIdThrowsException()
        {
            var id = Guid.NewGuid();
            var exception = new ApplicationException("Test Exception");
            _mockGateway.Setup(x => x.GetEntityById(id)).Throws(exception);
            Func<TenureResponseObject> throwException = () => _classUnderTest.Execute(id);
            throwException.Should().Throw<ApplicationException>().WithMessage("Test Exception");

        }
    }
}
