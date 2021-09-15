using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.Tests.V1.E2ETests.Fixtures;
using TenureInformationApi.Tests.V1.E2ETests.Steps;
using TestStack.BDDfy;
using Xunit;

namespace TenureInformationApi.Tests.V1.E2ETests.Stories
{
    [Story(
       AsA = "Internal Hackney user (such as a Housing Officer or Area housing Manager)",
       IWant = "the ability to capture a new tenure",
       SoThat = "I can create a new tenure in the system with all the relevant information")]
    [Collection("Aws collection")]
    public class PostTenureTests : IDisposable
    {
        private readonly AwsIntegrationTests<Startup> _dbFixture;
        private readonly TenureFixture _tenureFixture;
        private readonly PostTenureSteps _steps;

        public PostTenureTests(AwsIntegrationTests<Startup> dbFixture)
        {
            _dbFixture = dbFixture;
            _tenureFixture = new TenureFixture(_dbFixture.DynamoDbContext, _dbFixture.SimpleNotificationService);
            _steps = new PostTenureSteps(_dbFixture.Client);
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

        [Fact(Skip = "Test")]
        public void ServiceReturnsTheRequestedPerson()
        {
            this.Given(g => _tenureFixture.GivenNewTenureRequest())
                .When(w => _steps.WhenCreateTenureApiIsCalled(_tenureFixture.CreateTenureRequestObject))
                .Then(t => _steps.ThenTheTenureDetailsAreReturnedAndIdIsNotEmpty(_tenureFixture))
                .BDDfy();
        }

        [Fact(Skip = "Test")]
        public void ServiceReturnsBadRequestWhenTheyAreValidationErrors()
        {
            this.Given(g => _tenureFixture.GivenNewTenureRequestWithValidationErrors())
                .When(w => _steps.WhenCreateTenureApiIsCalled(_tenureFixture.CreateTenureRequestObject))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .And(t => _steps.ThenTheValidationErrorsAreReturned())
                .BDDfy();

        }
    }
}
