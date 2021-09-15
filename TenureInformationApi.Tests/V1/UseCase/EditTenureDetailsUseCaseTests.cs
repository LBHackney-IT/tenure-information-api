using AutoFixture;
using FluentAssertions;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.Tests.V1.Gateways;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Domain.Sns;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.Infrastructure;
using TenureInformationApi.V1.UseCase;
using Xunit;

namespace TenureInformationApi.Tests.V1.UseCase
{
    public class EditTenureDetailsUseCaseTests
    {
        private readonly Mock<ITenureGateway> _mockGateway;
        private readonly EditTenureDetailsUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<ISnsGateway> _tenureSnsGateway;
        private readonly TenureSnsFactory _tenureSnsFactory;

        public EditTenureDetailsUseCaseTests()
        {
            _mockGateway = new Mock<ITenureGateway>();
            _tenureSnsGateway = new Mock<ISnsGateway>();
            _tenureSnsFactory = new TenureSnsFactory();

            _classUnderTest = new EditTenureDetailsUseCase(_mockGateway.Object, _tenureSnsGateway.Object, _tenureSnsFactory);
        }

        [Fact]
        public async Task EditTenureDetailsUseCaseWhenTenureDoesntExistReturnsNull()
        {
            var mockQuery = _fixture.Create<TenureQueryRequest>();
            var mockRequestObject = _fixture.Create<EditTenureDetailsRequestObject>();
            var mockRawBody = "";
            var mockToken = _fixture.Create<Token>();

            _mockGateway.Setup(x => x.EditTenureDetails(mockQuery, mockRequestObject, mockRawBody)).ReturnsAsync((UpdateEntityResult<TenureInformationDb>) null);

            // call usecase method
            var response = await _classUnderTest.ExecuteAsync(mockQuery, mockRequestObject, mockRawBody, mockToken).ConfigureAwait(false);

            response.Should().BeNull();
        }

        [Fact]
        public async Task EditTenureDetailsUseCaseWhenTenureExistsReturnsTenureResponseObject()
        {
            var mockQuery = _fixture.Create<TenureQueryRequest>();
            var mockRequestObject = _fixture.Create<EditTenureDetailsRequestObject>();
            var mockRawBody = "";
            var mockToken = _fixture.Create<Token>();

            var mockResponseObject = _fixture.Create<TenureResponseObject>();

            var gatewayResponse = new UpdateEntityResult<TenureInformationDb>
            {
                UpdatedEntity = _fixture.Create<TenureInformationDb>()
            };

            _mockGateway.Setup(x => x.EditTenureDetails(mockQuery, mockRequestObject, mockRawBody)).ReturnsAsync(gatewayResponse);

            var response = await _classUnderTest.ExecuteAsync(mockQuery, mockRequestObject, mockRawBody, mockToken).ConfigureAwait(false);

            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(TenureResponseObject));

            response.StartOfTenureDate.Should().Be(gatewayResponse.UpdatedEntity.StartOfTenureDate);
            response.EndOfTenureDate.Should().Be(gatewayResponse.UpdatedEntity.EndOfTenureDate);
            response.TenureType.Code.Should().Be(gatewayResponse.UpdatedEntity.TenureType.Code);
        }

        [Fact]
        public async Task EditTenureDetailsUseCaseWhenNoChangesSNSGatewayIsntCalled()
        {
            var mockQuery = _fixture.Create<TenureQueryRequest>();
            var mockRequestObject = _fixture.Create<EditTenureDetailsRequestObject>();
            var mockRawBody = "";
            var mockToken = _fixture.Create<Token>();

            // setup mock gateway to return UpdateEntityResult with no changes
            var gatewayResult = MockUpdateEntityResultWhereNoChangesAreMade();

            _mockGateway
                .Setup(x => x.EditTenureDetails(It.IsAny<TenureQueryRequest>(), It.IsAny<EditTenureDetailsRequestObject>(), It.IsAny<string>()))
                .ReturnsAsync(gatewayResult);

            // call usecase method
            var response = await _classUnderTest.ExecuteAsync(mockQuery, mockRequestObject, mockRawBody, mockToken).ConfigureAwait(false);

            // assert result is TenureResponseObject
            response.Should().BeOfType(typeof(TenureResponseObject));

            // assert that sns factory wasnt called
            _tenureSnsGateway.Verify(x => x.Publish(It.IsAny<TenureSns>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task EditTenureDetailsUseCaseWhenChangesSNSGatewayIsCalled()
        {
            var mockQuery = _fixture.Create<TenureQueryRequest>();
            var mockRequestObject = _fixture.Create<EditTenureDetailsRequestObject>();
            var mockRawBody = "";
            var mockToken = _fixture.Create<Token>();

            // setup mock gateway to return UpdateEntityResult with no changes
            var gatewayResult = MockUpdateEntityResultWhereChangesAreMade();

            _mockGateway
                .Setup(x => x.EditTenureDetails(It.IsAny<TenureQueryRequest>(), It.IsAny<EditTenureDetailsRequestObject>(), It.IsAny<string>()))
                .ReturnsAsync(gatewayResult);

            // call usecase method
            var response = await _classUnderTest.ExecuteAsync(mockQuery, mockRequestObject, mockRawBody, mockToken).ConfigureAwait(false);

            // assert result is TenureResponseObject
            response.Should().BeOfType(typeof(TenureResponseObject));

            // assert that sns factory wasnt called
            _tenureSnsGateway.Verify(x => x.Publish(It.IsAny<TenureSns>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        private UpdateEntityResult<TenureInformationDb> MockUpdateEntityResultWhereChangesAreMade()
        {
            return new UpdateEntityResult<TenureInformationDb>
            {
                UpdatedEntity = _fixture.Create<TenureInformationDb>(),
                NewValues = new Dictionary<string, object>
                {
                    { "startOfTenureDate", _fixture.Create<DateTime>() }
                }
            };
        }

        private UpdateEntityResult<TenureInformationDb> MockUpdateEntityResultWhereNoChangesAreMade()
        {
            return new UpdateEntityResult<TenureInformationDb>
            {
                UpdatedEntity = _fixture.Create<TenureInformationDb>()
                // empty
            };
        }
    }
}
