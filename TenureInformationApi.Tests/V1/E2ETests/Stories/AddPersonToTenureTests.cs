using Hackney.Core.Testing.DynamoDb;
using Hackney.Core.Testing.Sns;
using System;
using TenureInformationApi.Tests.V1.E2ETests.Fixtures;
using TenureInformationApi.Tests.V1.E2ETests.Steps;
using TestStack.BDDfy;
using Xunit;

namespace TenureInformationApi.Tests.V1.E2ETests.Stories
{
    [Story(
       AsA = "Internal Hackney user (such as a Housing Officer or Area housing Manager)",
       IWant = "the ability to add a person to a tenure",
       SoThat = "I can create a link between a tenure and a person")]
    [Collection("AppTest collection")]
    public class AddPersonToTenureTests : IDisposable
    {
        private readonly IDynamoDbFixture _dbFixture;
        private readonly ISnsFixture _snsFixture;
        private readonly TenureFixture _tenureFixture;
        private readonly AddPersonToTenureStep _steps;

        public AddPersonToTenureTests(MockWebApplicationFactory<Startup> appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _snsFixture = appFactory.SnsFixture;
            _tenureFixture = new TenureFixture(_dbFixture.DynamoDbContext, _snsFixture.SimpleNotificationService);
            _steps = new AddPersonToTenureStep(appFactory.Client);
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
                if (null != _tenureFixture)
                    _tenureFixture.Dispose();
                if (null != _steps)
                    _steps.Dispose();

                _disposed = true;
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ServiceUpdateTheRequestedTenureWithNewHousehold(bool nullTenuredAssetType)
        {
            this.Given(g => _tenureFixture.GivenAnUpdateTenureWithNewHouseholdMemberRequest(nullTenuredAssetType))
                .When(w => _steps.WhenTheUpdateTenureApiIsCalled(_tenureFixture.TenureId, _tenureFixture.PersonId, _tenureFixture.UpdateTenureRequestObject))
                .Then(t => _steps.ThenANewHouseholdMemberIsAdded(_tenureFixture, _tenureFixture.PersonId, _tenureFixture.UpdateTenureRequestObject))
                .Then(t => _steps.ThenThePersonAddedToTenureEventIsRaised(_tenureFixture, _snsFixture))
                .BDDfy();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ServiceUpdatesTheRequestedUpdateTenureHouseHold(bool nullTenuredAssetType)
        {
            this.Given(g => _tenureFixture.GivenAnUpdateTenureHouseholdMemberRequest(nullTenuredAssetType))
                .When(w => _steps.WhenTheUpdateTenureApiIsCalled(_tenureFixture.TenureId, _tenureFixture.PersonId, _tenureFixture.UpdateTenureRequestObject))
                .Then(t => _steps.ThenTheHouseholdMemberTenureDetailsAreUpdated(_tenureFixture, _tenureFixture.PersonId, _tenureFixture.UpdateTenureRequestObject))
                .Then(t => _steps.ThenThePersonAddedToTenureEventIsRaised(_tenureFixture, _snsFixture))
                .BDDfy();
        }
        [Theory]
        [InlineData(null, false)]
        [InlineData(5, true)]
        public void ServiceReturnsConflictWhenIncorrectVersionNumber(int? versionNumber, bool nullTenuredAssetType)
        {
            this.Given(g => _tenureFixture.GivenAnUpdateTenureHouseholdMemberRequest(nullTenuredAssetType))
                .And(g => _steps.WhenTheUpdateTenureApiIsCalled(_tenureFixture.TenureId, _tenureFixture.PersonId, _tenureFixture.UpdateTenureRequestObject, versionNumber))
                .Then(t => _steps.ThenConflictIsReturned(versionNumber))
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsNotFoundIfPersonNotExist()
        {
            this.Given(g => _tenureFixture.GivenAUpdateTenureDoesNotExist())
                .When(w => _steps.WhenTheUpdateTenureApiIsCalled(_tenureFixture.TenureId, _tenureFixture.PersonId, _tenureFixture.UpdateTenureRequestObject))
                .Then(t => _steps.ThenNotFoundIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsBadRequestWhenTheyAreValidationErrors()
        {
            this.Given(g => _tenureFixture.GivenAnUpdateTenureRequestWithValidationError())
                .When(w => _steps.WhenTheUpdateTenureApiIsCalled(_tenureFixture.TenureId, _tenureFixture.PersonId, _tenureFixture.UpdateTenureRequestObject))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }
    }
}
