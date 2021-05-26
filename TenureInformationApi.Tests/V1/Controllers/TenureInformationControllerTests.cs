using System;
using TenureInformationApi.V1.Controllers;
using TenureInformationApi.V1.UseCase.Interfaces;
using NUnit.Framework;
using Moq;
using FluentAssertions;
using TenureInformationApi.V1.Boundary.Response;
using Microsoft.AspNetCore.Mvc;
using AutoFixture;
using TenureInformationApi.V1.Domain;
using System.Threading.Tasks;

namespace TenureInformationApi.Tests.V1.Controllers
{
    [TestFixture]
    public class TenureInformationControllerTests
    {
        private TenureInformationController _classUnderTest;
        private Mock<IGetByIdUseCase> _mockGetByIdUsecase;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _mockGetByIdUsecase = new Mock<IGetByIdUseCase>();
            _classUnderTest = new TenureInformationController(_mockGetByIdUsecase.Object);
        }

        [Test]
        public async Task GetTenureWithNoIdReturnsNotFound()
        {
            var id = Guid.NewGuid();
            _mockGetByIdUsecase.Setup(x => x.Execute(id)).ReturnsAsync((TenureResponseObject) null);

            var response = await _classUnderTest.GetByID(id).ConfigureAwait(false);
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(id);
        }

        [Test]
        public async Task GetTenureWithValidIdReturnsOKResponse()
        {
            var tenureResponse = _fixture.Create<TenureResponseObject>();
            _mockGetByIdUsecase.Setup(x => x.Execute(tenureResponse.Id)).ReturnsAsync(tenureResponse);

            var response = await _classUnderTest.GetByID(tenureResponse.Id).ConfigureAwait(false);
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().Be(tenureResponse);
        }
    }
}
