using AutoFixture;
using FluentAssertions;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Controllers;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Infrastructure;
using TenureInformationApi.V1.UseCase.Interfaces;
using Xunit;

namespace TenureInformationApi.Tests.V1.Controllers
{
    [Collection("LogCall collection")]
    public class TenureInformationControllerTests
    {
        private readonly TenureInformationController _classUnderTest;
        private readonly Mock<IGetByIdUseCase> _mockGetByIdUsecase;
        private readonly Mock<IPostNewTenureUseCase> _mockPostTenureUseCase;
        private readonly Mock<IUpdateTenureForPersonUseCase> _mockUpdateTenureForPersonUseCase;
        private readonly Mock<ITokenFactory> _mockTokenFactory;
        private readonly Mock<IHttpContextWrapper> _mockContextWrapper;
        private readonly Mock<HttpRequest> _mockHttpRequest;
        private readonly Fixture _fixture = new Fixture();

        public TenureInformationControllerTests()
        {
            var stubHttpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext(new ActionContext(stubHttpContext, new RouteData(), new ControllerActionDescriptor()));

            _mockGetByIdUsecase = new Mock<IGetByIdUseCase>();
            _mockPostTenureUseCase = new Mock<IPostNewTenureUseCase>();
            _mockUpdateTenureForPersonUseCase = new Mock<IUpdateTenureForPersonUseCase>();
            _mockTokenFactory = new Mock<ITokenFactory>();
            _mockContextWrapper = new Mock<IHttpContextWrapper>();
            _mockHttpRequest = new Mock<HttpRequest>();
            _classUnderTest = new TenureInformationController(_mockGetByIdUsecase.Object, _mockPostTenureUseCase.Object, _mockUpdateTenureForPersonUseCase.Object, _mockTokenFactory.Object,
                _mockContextWrapper.Object);
            _classUnderTest.ControllerContext = controllerContext;

            _mockContextWrapper.Setup(x => x.GetContextRequestHeaders(It.IsAny<HttpContext>())).Returns(new HeaderDictionary());

        }

        private TenureQueryRequest ConstructRequest(Guid? id = null)
        {
            return new TenureQueryRequest() { Id = id ?? Guid.NewGuid() };
        }

        private TenureQueryRequest ConstructQuery()
        {
            return new TenureQueryRequest() { Id = Guid.NewGuid() };
        }

        private UpdateTenureRequest ConstructUpdateQuery()
        {
            return new UpdateTenureRequest() { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() };
        }

        private UpdateTenureForPersonRequestObject ConstructUpdateRequest()
        {
            var request = _fixture.Create<UpdateTenureForPersonRequestObject>();

            return request;
        }

        [Fact]
        public async Task GetTenureWithNoIdReturnsNotFound()
        {
            var request = ConstructRequest();
            _mockGetByIdUsecase.Setup(x => x.Execute(request)).ReturnsAsync((TenureInformation) null);

            var response = await _classUnderTest.GetByID(request).ConfigureAwait(false);
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(request.Id);
        }

        [Fact]
        public async Task GetTenureWithValidIdReturnsOKResponse()
        {
            var tenureResponse = _fixture.Create<TenureInformation>();
            var request = ConstructRequest(tenureResponse.Id);
            _mockGetByIdUsecase.Setup(x => x.Execute(request)).ReturnsAsync(tenureResponse);

            var response = await _classUnderTest.GetByID(request).ConfigureAwait(false);
            response.Should().BeOfType(typeof(OkObjectResult));
            _classUnderTest.HttpContext.Response.Headers.TryGetValue(HeaderConstants.ETag, out StringValues val).Should().BeTrue();
            val.First().Should().Be(tenureResponse.VersionNumber.ToString());
            (response as OkObjectResult).Value.Should().BeEquivalentTo(tenureResponse.ToResponse());
        }

        [Fact]
        public async Task PostNewTenureIdAsyncFoundReturnsResponse()
        {
            // Arrange
            var tenureResponse = _fixture.Create<TenureResponseObject>();
            _mockPostTenureUseCase.Setup(x => x.ExecuteAsync(It.IsAny<CreateTenureRequestObject>(), It.IsAny<Token>()))
                .ReturnsAsync(tenureResponse);

            // Act
            var response = await _classUnderTest.PostNewTenure(new CreateTenureRequestObject()).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(CreatedResult));
            (response as CreatedResult).Value.Should().Be(tenureResponse);
        }

        [Fact]
        public void PostNewTenureIdAsyncExceptionIsThrown()
        {
            // Arrange
            var exception = new ApplicationException("Test exception");
            _mockPostTenureUseCase.Setup(x => x.ExecuteAsync(It.IsAny<CreateTenureRequestObject>(), It.IsAny<Token>()))
                                 .ThrowsAsync(exception);

            // Act
            Func<Task<IActionResult>> func = async () => await _classUnderTest.PostNewTenure(new CreateTenureRequestObject())
                .ConfigureAwait(false);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }

        [Fact]
        public async Task UpdateTenureForPersonAsyncFoundReturnsFound()
        {
            // Arrange
            var query = ConstructUpdateQuery();
            var request = ConstructUpdateRequest();
            var personResponse = _fixture.Create<TenureResponseObject>();
            _mockUpdateTenureForPersonUseCase.Setup(x => x.ExecuteAsync(query, request, It.IsAny<Token>()))
                                    .ReturnsAsync(personResponse);

            // Act
            var response = await _classUnderTest.UpdateTenureForPerson(query, request).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(NoContentResult));
        }

        [Fact]
        public async Task UpdateTenureForPersonAsyncNotFoundReturnsNotFound()
        {
            // Arrange
            var query = ConstructUpdateQuery();
            var request = ConstructUpdateRequest();
            _mockUpdateTenureForPersonUseCase.Setup(x => x.ExecuteAsync(query, request, It.IsAny<Token>()))
                                    .ReturnsAsync((TenureResponseObject) null);

            // Act
            var response = await _classUnderTest.UpdateTenureForPerson(query, request).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(query.Id);
        }

        [Fact]
        public void UpdateTenureForPersonAsyncExceptionIsThrown()
        {
            // Arrange
            var query = ConstructUpdateQuery();
            var exception = new ApplicationException("Test exception");
            _mockUpdateTenureForPersonUseCase.Setup(x => x.ExecuteAsync(query, It.IsAny<UpdateTenureForPersonRequestObject>(), It.IsAny<Token>()))
                                    .ThrowsAsync(exception);

            // Act
            Func<Task<IActionResult>> func = async () => await _classUnderTest.UpdateTenureForPerson(query, new UpdateTenureForPersonRequestObject())
                .ConfigureAwait(false);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }
    }
}
