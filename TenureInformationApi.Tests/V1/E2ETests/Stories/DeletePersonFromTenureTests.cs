using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.Tests.V1.E2ETests.Fixtures;
using TenureInformationApi.Tests.V1.E2ETests.Steps;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Domain;
using TestStack.BDDfy;
using Xunit;

namespace TenureInformationApi.Tests.V1.E2ETests.Stories

{
    [Story(
        AsA = "Internal Hackney user (such as a Housing Officer or Area housing Manager)",
        IWant = "the ability to delete a person from a tenure",
        SoThat = " can end their relationship to the legal tenure"
    )]
    [Collection("Aws collection")]
    public class DeletePersonFromTenureTests : IDisposable
    {
        private readonly AwsIntegrationTests<Startup> _dbFixture;

        private readonly TenureFixture _tenureFixture;
        private readonly DeleteTenureDetailsStep _steps;
        private readonly Fixture _fixture = new Fixture();

        public DeletePersonFromTenureTests(AwsIntegrationTests<Startup> dbFixture)
        {
            _dbFixture = dbFixture;
            _tenureFixture = new TenureFixture(_dbFixture.DynamoDbContext, _dbFixture.SimpleNotificationService);
            _steps = new DeleteTenureDetailsStep(_dbFixture.Client);
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

            var query = _fixture.Create<DeletePersonFromTenureQueryRequest>();



            // tenure and person exist
            this.Given(g => _tenureFixture.GivenATenureExistsWithManyHouseholdMembers())
               .When(w => _steps.WhenDeletePersonFromTenureApiIsCalledAsync(new DeletePersonFromTenureQueryRequest
               {
                   TenureId = _tenureFixture.TenureId,
                   PersonId = _tenureFixture.Tenure.HouseholdMembers.First().Id
               }))
               .Then(t => _steps.NoContentResponseReturned())
               .And(a => _steps.PersonRemovedFromTenure(_tenureFixture.TenureId, _tenureFixture.Tenure.HouseholdMembers.First().Id, _tenureFixture))
               .BDDfy();
        }

    }
}
