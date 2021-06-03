using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Controllers;
using TenureInformationApi.V1.UseCase.Interfaces;
using Xunit;

namespace TenureInformationApi.Tests.V1.Controllers
{
    [Collection("LogCall collection")]
    public class TenureInformationControllerTests
    {
        private readonly TenureInformationController _classUnderTest;
        private readonly Mock<IGetByIdUseCase> _mockGetByIdUsecase;
        private readonly Fixture _fixture = new Fixture();

        public TenureInformationControllerTests()
        {
            _mockGetByIdUsecase = new Mock<IGetByIdUseCase>();
            _classUnderTest = new TenureInformationController(_mockGetByIdUsecase.Object);
        }

        private GetByIdRequest ConstructRequest(Guid? id = null)
        {
            return new GetByIdRequest() { Id = id ?? Guid.NewGuid() };
        }

        [Fact]
        public async Task GetTenureWithNoIdReturnsNotFound()
        {
            var request = ConstructRequest();
            _mockGetByIdUsecase.Setup(x => x.Execute(request)).ReturnsAsync((TenureResponseObject) null);

            var response = await _classUnderTest.GetByID(request).ConfigureAwait(false);
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(request.Id);
        }

        [Fact]
        public async Task GetTenureWithValidIdReturnsOKResponse()
        {
            var tenureResponse = _fixture.Create<TenureResponseObject>();
            var request = ConstructRequest(tenureResponse.Id);
            _mockGetByIdUsecase.Setup(x => x.Execute(request)).ReturnsAsync(tenureResponse);

            var response = await _classUnderTest.GetByID(request).ConfigureAwait(false);
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().Be(tenureResponse);
        }
    }
}
