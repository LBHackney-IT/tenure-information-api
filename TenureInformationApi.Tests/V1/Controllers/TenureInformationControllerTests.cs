using AutoFixture;
using FluentAssertions;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using Microsoft.AspNetCore.Http;
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
        private readonly Mock<IPostNewTenureUseCase> _mockPostTenureUseCase;
        private readonly Mock<IUpdateTenureUseCase> _mockUpdateTenureUseCase;
        private readonly Mock<ITokenFactory> _mockTokenFactory;
        private readonly Mock<IHttpContextWrapper> _mockContextWrapper;
        private readonly Mock<HttpRequest> _mockHttpRequest;
        private readonly Fixture _fixture = new Fixture();

        public TenureInformationControllerTests()
        {
            _mockGetByIdUsecase = new Mock<IGetByIdUseCase>();
            _mockPostTenureUseCase = new Mock<IPostNewTenureUseCase>();
            _mockUpdateTenureUseCase = new Mock<IUpdateTenureUseCase>();
            _mockTokenFactory = new Mock<ITokenFactory>();
            _mockContextWrapper = new Mock<IHttpContextWrapper>();
            _mockHttpRequest = new Mock<HttpRequest>();
            _classUnderTest = new TenureInformationController(_mockGetByIdUsecase.Object, _mockPostTenureUseCase.Object, _mockUpdateTenureUseCase.Object, _mockTokenFactory.Object,
                _mockContextWrapper.Object);
            _mockContextWrapper.Setup(x => x.GetContextRequestHeaders(It.IsAny<HttpContext>())).Returns(new HeaderDictionary());

        }

        private TenureQueryRequest ConstructRequest(Guid? id = null)
        {
            return new TenureQueryRequest() { Id = id ?? Guid.NewGuid() };
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
        public async Task UpdateTenureByIdAsyncFoundReturnsFound()
        {
            // Arrange
            var query = ConstructQuery();
            var request = ConstructUpdateRequest();
            var personResponse = _fixture.Create<TenureResponseObject>();
            _mockUpdateTenureUseCase.Setup(x => x.ExecuteAsync(query, request))
                                    .ReturnsAsync(personResponse);

            // Act
            var response = await _classUnderTest.UpdateTenure(query, request).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(NoContentResult));
        }

        [Fact]
        public async Task UpdateTenureByIdAsyncNotFoundReturnsNotFound()
        {
            // Arrange
            var query = ConstructQuery();
            var request = ConstructUpdateRequest();
            var personResponse = _fixture.Create<TenureResponseObject>();
            _mockUpdateTenureUseCase.Setup(x => x.ExecuteAsync(query, request))
                                    .ReturnsAsync((TenureResponseObject) null);

            // Act
            var response = await _classUnderTest.UpdateTenure(query, request).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(query.Id);

        }

        [Fact]
        public void UpdateTenureByIdAsyncExceptionIsThrown()
        {
            // Arrange
            var query = ConstructQuery();
            var exception = new ApplicationException("Test exception");
            _mockUpdateTenureUseCase.Setup(x => x.ExecuteAsync(query, It.IsAny<UpdateTenureRequestObject>()))
                                    .ThrowsAsync(exception);

            // Act
            Func<Task<IActionResult>> func = async () => await _classUnderTest.UpdateTenure(query, new UpdateTenureRequestObject())
                .ConfigureAwait(false);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }
    }
}
