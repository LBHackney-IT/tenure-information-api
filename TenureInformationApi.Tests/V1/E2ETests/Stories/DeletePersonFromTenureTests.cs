using AutoFixture;
using Hackney.Core.Testing.DynamoDb;
using Hackney.Core.Testing.Sns;
using Hackney.Shared.Tenure.Boundary.Requests;
using System;
using System.Linq;
using TenureInformationApi.Tests.V1.E2ETests.Fixtures;
using TenureInformationApi.Tests.V1.E2ETests.Steps;
using TestStack.BDDfy;
using Xunit;

namespace TenureInformationApi.Tests.V1.E2ETests.Stories
{
    [Story(
        AsA = "Internal Hackney user (such as a Housing Officer or Area housing Manager)",
        IWant = "the ability to delete a person from a tenure",
        SoThat = " can end their relationship to the legal tenure"
    )]
    [Collection("AppTest collection")]
    public class DeletePersonFromTenureTests : IDisposable
    {
        private readonly IDynamoDbFixture _dbFixture;
        private readonly ISnsFixture _snsFixture;
        private readonly TenureFixture _tenureFixture;
        private readonly DeletePersonFromTenureStep _steps;
        private readonly Fixture _fixture = new Fixture();

        public DeletePersonFromTenureTests(MockWebApplicationFactory<Startup> appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _snsFixture = appFactory.SnsFixture;
            _tenureFixture = new TenureFixture(_dbFixture.DynamoDbContext, _snsFixture.SimpleNotificationService);
            _steps = new DeletePersonFromTenureStep(appFactory.Client);
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

        [Fact]
        public void ServiceReturns404WhenTenureDoesntExist()
        {
            var query = _fixture.Create<DeletePersonFromTenureQueryRequest>();

            this.Given(g => _tenureFixture.GivenNoTenuresExist())
                .When(w => _steps.WhenDeletePersonFromTenureApiIsCalledAsync(query))
                .Then(t => _steps.NotFoundResponseReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturns404WhenPersonDoesntExistInTenure()
        {
            this.Given(g => _tenureFixture.GivenATenureExistsWithNoHouseholdMembers())
                .When(w => _steps.WhenDeletePersonFromTenureApiIsCalledAsync(new DeletePersonFromTenureQueryRequest
                {
                    TenureId = _tenureFixture.TenureId,
                    PersonId = _fixture.Create<Guid>()
                }))
                .Then(t => _steps.NotFoundResponseReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturns204WhenPersonWasRemovedFromTenure()
        {
            // tenure and person exist
            this.Given(g => _tenureFixture.GivenATenureExistsWithManyHouseholdMembers())
                .When(w => _steps.WhenDeletePersonFromTenureApiIsCalledAsync(new DeletePersonFromTenureQueryRequest
                {
                    TenureId = _tenureFixture.TenureId,
                    PersonId = _tenureFixture.Tenure.HouseholdMembers.First().Id
                }))
                .Then(t => _steps.NoContentResponseReturned())
                .And(a => _steps.PersonRemovedFromTenure(_tenureFixture.TenureId, _tenureFixture.Tenure.HouseholdMembers.First().Id, _tenureFixture))
                .And(t => _steps.ThenThePersonRemovedFromTenureEventIsRaised(_tenureFixture, _snsFixture))
                .BDDfy();
        }
    }
}
