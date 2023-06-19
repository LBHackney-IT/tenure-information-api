using AutoFixture;
using Hackney.Core.Testing.DynamoDb;
using Hackney.Core.Testing.Sns;
using Hackney.Shared.Tenure.Boundary.Requests;
using System;
using TenureInformationApi.Tests.V1.E2ETests.Constants;
using TenureInformationApi.Tests.V1.E2ETests.Fixtures;
using TenureInformationApi.Tests.V1.E2ETests.Steps;
using TestStack.BDDfy;
using Xunit;

namespace TenureInformationApi.Tests.V1.E2ETests.Stories
{
    [Story(
        AsA = "Internal Hackney user (such as a Housing Officer or Area housing Manager)",
        IWant = "the ability to edit details of a tenure",
        SoThat = "I can ensure that the tenure details are up to date"
    )]
    [Collection("AppTest collection")]
    public class EditTenureDetailsTests : IDisposable
    {
        private readonly IDynamoDbFixture _dbFixture;
        private readonly ISnsFixture _snsFixture;
        private readonly TenureFixture _tenureFixture;
        private readonly EditTenureDetailsStep _steps;
        private readonly Fixture _fixture = new Fixture();

        public EditTenureDetailsTests(MockWebApplicationFactory<Startup> appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _snsFixture = appFactory.SnsFixture;
            _tenureFixture = new TenureFixture(_dbFixture.DynamoDbContext, _snsFixture.SimpleNotificationService);
            _steps = new EditTenureDetailsStep(appFactory.Client);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _tenureFixture?.Dispose();
                _steps?.Dispose();
                _snsFixture?.PurgeAllQueueMessages();

                _disposed = true;
            }
        }

        [Fact]
        public void ServiceReturns400BadRequest()
        {
            var invalidRequestObject = CreateInvalidRequestObject();

            this.Given(g => _tenureFixture.GivenATenureExist(false))
                .When(w => _steps.WhenEditTenureDetailsApiIsCalled(_tenureFixture.TenureId, invalidRequestObject))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .And(t => _steps.ThenTheValidationErrorsAreReturned("EndOfTenureDate"))
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsCustomEditTenureDetailsBadRequestResponse()
        {
            var tenureStartDate = DateTime.UtcNow.AddYears(-1);
            var tenureEndDate = tenureStartDate.AddDays(-7); // end date that is less than start date

            var requestObject = new
            {
                EndOfTenureDate = tenureEndDate
            };

            this.Given(g => _tenureFixture.GivenATenureExistWithNoEndDate(tenureStartDate))
                .When(w => _steps.WhenEditTenureDetailsApiIsCalled(_tenureFixture.TenureId, requestObject))
                .Then(t => _steps.ThenCustomEditTenureDetailsBadRequestIsReturned())
                .And(t => _steps.ThenTheValidationErrorsAreReturned("EndOfTenureDate"))
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsNotFoundResponse()
        {
            var randomId = Guid.NewGuid();
            var requestObject = CreateValidRequestObject();

            this.Given(g => _tenureFixture.GivenATenureDoesNotExist())
                .When(w => _steps.WhenEditTenureDetailsApiIsCalled(randomId, requestObject))
                .Then(t => _steps.ThenNotFoundIsReturned())
                .BDDfy();
        }


        [Theory]
        [InlineData(false, AuthenticationConstants.E2EToken)]
        [InlineData(true, AuthenticationConstants.E2ETokenEditCharges)]
        [InlineData(false, AuthenticationConstants.E2ETokenEditCharges)]
        public void ServiceReturns204AndUpdatesDatabase(bool includeCharges, string token)
        {
            var requestObject = CreateValidRequestObject(includeCharges);

            this.Given(g => _tenureFixture.GivenATenureExist(false))
                .When(w => _steps.WhenEditTenureDetailsApiIsCalled(_tenureFixture.TenureId, requestObject, default(int), token))
                .Then(t => _steps.ThenNoContentResponseReturned())
                .And(a => _steps.TheTenureHasBeenUpdatedInTheDatabase(_tenureFixture, requestObject))
                .And(t => _steps.ThenTheTenureUpdatedEventIsRaised(_tenureFixture, _snsFixture))
                .BDDfy();
        }

        [Fact]
        public void ServiceDoesNotEditChargesWhenUserInIncorrectGroup()
        {
            var requestObject = CreateValidRequestObject(true);

            var token = AuthenticationConstants.E2EToken;

            this.Given(g => _tenureFixture.GivenATenureExist(false))
                .When(w => _steps.WhenEditTenureDetailsApiIsCalled(_tenureFixture.TenureId, requestObject, default(int), token))
                .Then(t => _steps.ThenUnauthorizedIsReturned())
                .BDDfy();
        }


        [Theory]
        [InlineData(null)]
        [InlineData(5)]
        public void ServiceReturnsConflictWhenIncorrectVersionNumber(int? versionNumber)
        {
            var requestObject = CreateValidRequestObject();

            this.Given(g => _tenureFixture.GivenATenureExist(false))
                .When(w => _steps.WhenEditTenureDetailsApiIsCalled(_tenureFixture.TenureId, requestObject, versionNumber))
                .Then(t => _steps.ThenConflictIsReturned(versionNumber))
                .BDDfy();
        }

        private EditTenureDetailsRequestObject CreateValidRequestObject(bool includeCharges = false)
        {
            var tenureStartDate = DateTime.UtcNow.AddYears(-1);
            var tenureEndDate = tenureStartDate.AddDays(150);

            var composer = _fixture.Build<EditTenureDetailsRequestObject>()
                .With(x => x.StartOfTenureDate, tenureStartDate)
                .With(x => x.EndOfTenureDate, tenureEndDate);
            if (!includeCharges)
            {
                composer = composer.Without(x => x.Charges);
            }
               
            return composer.Create();
        }

        private EditTenureDetailsRequestObject CreateInvalidRequestObject(bool includeCharges = false)
        {
            var tenureStartDate = DateTime.UtcNow.AddYears(-1);
            var tenureEndDate = tenureStartDate.AddDays(-7);

            var composer = _fixture.Build<EditTenureDetailsRequestObject>()
                .With(x => x.StartOfTenureDate, tenureStartDate)
                .With(x => x.EndOfTenureDate, tenureEndDate);

            if (!includeCharges)
            {
                composer = composer.Without(x => x.Charges);
            }
                
            return composer.Create();
        }
    }
}
