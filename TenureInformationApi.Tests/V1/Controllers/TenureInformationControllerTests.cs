using AutoFixture;
using FluentAssertions;
using FluentValidation.Results;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Controllers;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Infrastructure;
using TenureInformationApi.V1.Infrastructure.Exceptions;
using TenureInformationApi.V1.UseCase.Interfaces;
using Xunit;

namespace TenureInformationApi.Tests.V1.Controllers
{
    [Collection("LogCall collection")]
    public class TenureInformationControllerTests
    {
        private readonly Mock<IGetByIdUseCase> _mockGetByIdUsecase;
        private readonly Mock<IPostNewTenureUseCase> _mockPostTenureUseCase;
        private readonly Mock<IUpdateTenureForPersonUseCase> _mockUpdateTenureForPersonUseCase;
        private readonly Mock<IEditTenureDetailsUseCase> _mockEditTenureDetailsUseCase;
        private readonly Mock<IDeletePersonFromTenureUseCase> _mockDeletePersonFromTenureUseCase;

        private readonly Mock<ITokenFactory> _mockTokenFactory;
        private readonly Mock<IHttpContextWrapper> _mockContextWrapper;

        private readonly Mock<HttpRequest> _mockHttpRequest;
        private readonly HeaderDictionary _requestHeaders;
        private readonly Mock<HttpResponse> _mockHttpResponse;
        private readonly HeaderDictionary _responseHeaders;

        private readonly TenureInformationController _classUnderTest;
        private readonly Fixture _fixture = new Fixture();
        private readonly Random _random = new Random();

        private const string RequestBodyText = "Some request body text";

        public TenureInformationControllerTests()
        {
            _mockGetByIdUsecase = new Mock<IGetByIdUseCase>();
            _mockPostTenureUseCase = new Mock<IPostNewTenureUseCase>();
            _mockUpdateTenureForPersonUseCase = new Mock<IUpdateTenureForPersonUseCase>();
            _mockEditTenureDetailsUseCase = new Mock<IEditTenureDetailsUseCase>();
            _mockDeletePersonFromTenureUseCase = new Mock<IDeletePersonFromTenureUseCase>();

            _mockTokenFactory = new Mock<ITokenFactory>();
            _mockContextWrapper = new Mock<IHttpContextWrapper>();
            _mockHttpRequest = new Mock<HttpRequest>();
            _mockHttpResponse = new Mock<HttpResponse>();

            _classUnderTest = new TenureInformationController(
                _mockGetByIdUsecase.Object,
                _mockPostTenureUseCase.Object,
                _mockUpdateTenureForPersonUseCase.Object,
                _mockEditTenureDetailsUseCase.Object,
                _mockDeletePersonFromTenureUseCase.Object,
                _mockTokenFactory.Object,
                _mockContextWrapper.Object);

            // changes to allow reading of raw request body
            _mockHttpRequest.SetupGet(x => x.Body).Returns(new MemoryStream(Encoding.Default.GetBytes(RequestBodyText)));

            _requestHeaders = new HeaderDictionary();
            _mockHttpRequest.SetupGet(x => x.Headers).Returns(_requestHeaders);

            _mockContextWrapper
                .Setup(x => x.GetContextRequestHeaders(It.IsAny<HttpContext>()))
                .Returns(_requestHeaders);

            _responseHeaders = new HeaderDictionary();
            _mockHttpResponse.SetupGet(x => x.Headers).Returns(_responseHeaders);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            mockHttpContext.SetupGet(x => x.Response).Returns(_mockHttpResponse.Object);

            var controllerContext = new ControllerContext(new ActionContext(mockHttpContext.Object, new RouteData(), new ControllerActionDescriptor()));
            _classUnderTest.ControllerContext = controllerContext;
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
            (response as OkObjectResult).Value.Should().BeEquivalentTo(tenureResponse.ToResponse());

            var expectedEtagValue = $"\"{tenureResponse.VersionNumber}\"";
            _classUnderTest.HttpContext.Response.Headers.TryGetValue(HeaderConstants.ETag, out StringValues val).Should().BeTrue();
            val.First().Should().Be(expectedEtagValue);
        }

        [Fact]
        public async Task GetTenureWhenVersionNumberIsNullReturnsEmptyETAG()
        {
            // when versionNumber is null, ETAG is set to 0

            // Arrange
            var mockTenureResponse = _fixture.Build<TenureInformation>()
                .With(x => x.VersionNumber, (int?) null)
                .Create();

            var mockRequest = ConstructRequest(mockTenureResponse.Id);

            _mockGetByIdUsecase.Setup(x => x.Execute(mockRequest)).ReturnsAsync(mockTenureResponse);

            // Act
            var response = await _classUnderTest.GetByID(mockRequest).ConfigureAwait(false);

            // Assert ETAG value is 0, no error thrown
            var expectedEtagValue = $"\"\"";
            _classUnderTest.HttpContext.Response.Headers.TryGetValue(HeaderConstants.ETag, out StringValues val).Should().BeTrue();
            val.First().Should().Be(expectedEtagValue);
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
            _mockUpdateTenureForPersonUseCase.Setup(x => x.ExecuteAsync(query, request, It.IsAny<Token>(), It.IsAny<int?>()))
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
            _mockUpdateTenureForPersonUseCase.Setup(x => x.ExecuteAsync(query, request, It.IsAny<Token>(), It.IsAny<int?>()))
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
            _mockUpdateTenureForPersonUseCase.Setup(x => x.ExecuteAsync(query, It.IsAny<UpdateTenureForPersonRequestObject>(), It.IsAny<Token>(), It.IsAny<int?>()))
                                    .ThrowsAsync(exception);

            // Act
            Func<Task<IActionResult>> func = async () => await _classUnderTest.UpdateTenureForPerson(query, new UpdateTenureForPersonRequestObject())
                .ConfigureAwait(false);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }

        [Theory]
        [InlineData(null, 0)]
        [InlineData(0, 1)]
        [InlineData(0, null)]
        [InlineData(2, 1)]
        public async Task UpdatePersonByIdAsyncVersionNumberConflictExceptionReturns409(int? expected, int? actual)
        {
            // Arrange
            var query = ConstructUpdateQuery();

            _requestHeaders.Add(HeaderConstants.IfMatch, new StringValues(expected?.ToString()));

            var exception = new VersionNumberConflictException(expected, actual);
            _mockUpdateTenureForPersonUseCase.Setup(x => x.ExecuteAsync(query, It.IsAny<UpdateTenureForPersonRequestObject>(), It.IsAny<Token>(), expected))
                                    .ThrowsAsync(exception);

            // Act
            var result = await _classUnderTest.UpdateTenureForPerson(query, new UpdateTenureForPersonRequestObject()).ConfigureAwait(false);

            // Assert
            result.Should().BeOfType(typeof(ConflictObjectResult));
            (result as ConflictObjectResult).Value.Should().BeEquivalentTo(exception.Message);
        }

        [Fact]
        public async Task EditTenureDetailsWhenTenureDoesntExistReturns404NotFoundResponse()
        {
            var mockQuery = _fixture.Create<TenureQueryRequest>();
            var mockRequestObject = _fixture.Create<EditTenureDetailsRequestObject>();

            _mockEditTenureDetailsUseCase.Setup(x => x.ExecuteAsync(It.IsAny<TenureQueryRequest>(), It.IsAny<EditTenureDetailsRequestObject>(), It.IsAny<string>(), It.IsAny<Token>(), It.IsAny<int?>())).ReturnsAsync((TenureResponseObject) null);

            var response = await _classUnderTest.EditTenureDetails(mockQuery, mockRequestObject).ConfigureAwait(false);

            response.Should().BeOfType(typeof(NotFoundResult));
        }

        [Fact]
        public async Task EditTenureDetailsWhenValidReturns204NoContentResponse()
        {
            var mockQuery = _fixture.Create<TenureQueryRequest>();
            var mockRequestObject = _fixture.Create<EditTenureDetailsRequestObject>();

            _mockEditTenureDetailsUseCase.Setup(x => x.ExecuteAsync(It.IsAny<TenureQueryRequest>(), It.IsAny<EditTenureDetailsRequestObject>(), It.IsAny<string>(), It.IsAny<Token>(), It.IsAny<int?>())).ReturnsAsync(_fixture.Create<TenureResponseObject>());

            var response = await _classUnderTest.EditTenureDetails(mockQuery, mockRequestObject).ConfigureAwait(false);

            response.Should().BeOfType(typeof(NoContentResult));
        }

        [Fact]
        public async Task EditTenureDetailsWhenExceptionIsThrownReturnsCustomEditTenureDetailsBadRequest()
        {
            // Arrange
            var tenureStartDate = _fixture.Create<DateTime>();
            var tenureEndDate = tenureStartDate.AddDays(-7);

            var mockQuery = _fixture.Create<TenureQueryRequest>();
            var mockRequestObject = new EditTenureDetailsRequestObject { StartOfTenureDate = tenureEndDate };

            var numberOfErrors = _random.Next(2, 5);
            var mockValidationResult = new ValidationResult(_fixture.CreateMany<ValidationFailure>(numberOfErrors));

            // setup usecase to throw custom exception
            _mockEditTenureDetailsUseCase
                .Setup(x => x.ExecuteAsync(It.IsAny<TenureQueryRequest>(), It.IsAny<EditTenureDetailsRequestObject>(), It.IsAny<string>(), It.IsAny<Token>(), It.IsAny<int?>()))
                .Throws(new EditTenureInformationValidationException(mockValidationResult));

            // Act
            var response = await _classUnderTest.EditTenureDetails(mockQuery, mockRequestObject).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(BadRequestObjectResult));
            ((response as BadRequestObjectResult).Value as CustomEditTenureDetailsBadRequestResponse).Errors.Should().HaveCount(numberOfErrors);
        }

        [Theory]
        [InlineData(null, 0)]
        [InlineData(0, 1)]
        [InlineData(0, null)]
        [InlineData(2, 1)]
        public async Task EditTenureDetailsVersionNumberConflictExceptionReturns409(int? expected, int? actual)
        {
            // Arrange
            var mockQuery = _fixture.Create<TenureQueryRequest>();
            var mockRequestObject = new EditTenureDetailsRequestObject();

            _requestHeaders.Add(HeaderConstants.IfMatch, $"\"{new StringValues(expected?.ToString())}\"");

            var exception = new VersionNumberConflictException(expected, actual);

            _mockEditTenureDetailsUseCase
                .Setup(x => x.ExecuteAsync(It.IsAny<TenureQueryRequest>(), It.IsAny<EditTenureDetailsRequestObject>(), It.IsAny<string>(), It.IsAny<Token>(), It.IsAny<int?>()))
                .ThrowsAsync(exception);

            // Act
            var response = await _classUnderTest.EditTenureDetails(mockQuery, mockRequestObject).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(ConflictObjectResult));
            (response as ConflictObjectResult).Value.Should().BeEquivalentTo(exception.Message);
        }

        [Fact]
        public async Task DeletePersonFromTenureWhenTenureDoesntExistReturnsNotFound()
        {
            // Arrange
            var mockQuery = _fixture.Create<DeletePersonFromTenureQueryRequest>();

            var exception = new TenureNotFoundException();

            _mockDeletePersonFromTenureUseCase
                .Setup(x => x.Execute(It.IsAny<DeletePersonFromTenureQueryRequest>()))
                .ThrowsAsync(exception);

            // Act
            var result = await _classUnderTest.DeletePersonFromTenure(mockQuery).ConfigureAwait(false);

            // Assert
            result.Should().BeOfType(typeof(NotFoundObjectResult));
            (result as NotFoundObjectResult).Value.Should().Be(mockQuery.TenureId);
        }

        [Fact]
        public async Task DeletePersonFromTenureWhenPersonDoesntExistInTenureReturnsNotFound()
        {
            // Arrange
            var mockQuery = _fixture.Create<DeletePersonFromTenureQueryRequest>();

            var exception = new PersonNotFoundInTenureException();

            _mockDeletePersonFromTenureUseCase
                .Setup(x => x.Execute(It.IsAny<DeletePersonFromTenureQueryRequest>()))
                .ThrowsAsync(exception);

            // Act
            var result = await _classUnderTest.DeletePersonFromTenure(mockQuery).ConfigureAwait(false);

            // Assert
            result.Should().BeOfType(typeof(NotFoundObjectResult));
            (result as NotFoundObjectResult).Value.Should().Be(mockQuery.PersonId);
        }

        [Fact]
        public async Task DeletePersonFromTenureWhenPersonExistsReturnsNoContentResponseAsync()
        {
            // Arrange
            var mockQuery = _fixture.Create<DeletePersonFromTenureQueryRequest>();

            // Act
            var result = await _classUnderTest.DeletePersonFromTenure(mockQuery).ConfigureAwait(false);

            // Assert
            result.Should().BeOfType(typeof(NoContentResult));
        }
    }
}
