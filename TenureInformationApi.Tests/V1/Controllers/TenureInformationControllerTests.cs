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
        public void GetTenureWithNoIdReturnsNotFound()
        {
            var id = Guid.NewGuid();
            _mockGetByIdUsecase.Setup(x => x.Execute(id)).Returns((TenureResponseObject) null);

            var response = _classUnderTest.GetByID(id) as NotFoundObjectResult;
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            response.StatusCode.Should().Be(404);
        }

        [Test]
        public void GetTenureWithValidIdReturnsOKResponse()
        {
            var tenureResponse = _fixture.Create<TenureResponseObject>();
            _mockGetByIdUsecase.Setup(x => x.Execute(tenureResponse.Id)).Returns(tenureResponse);

            var response = _classUnderTest.GetByID(tenureResponse.Id) as OkObjectResult;
            response.Value.Should().Be(tenureResponse);
            response.StatusCode.Should().Be(200);
        }
    }
}
