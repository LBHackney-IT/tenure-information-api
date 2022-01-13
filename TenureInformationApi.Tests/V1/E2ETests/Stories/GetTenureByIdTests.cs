using Hackney.Core.Testing.DynamoDb;
using System;
using TenureInformationApi.Tests.V1.E2ETests.Fixtures;
using TenureInformationApi.Tests.V1.E2ETests.Steps;
using TestStack.BDDfy;
using Xunit;

namespace TenureInformationApi.Tests.V1.E2ETests.Stories
{
    [Story(
         AsA = "Service",
         IWant = "an endpoint to return person details",
         SoThat = "it is possible to view the details of a person")]
    [Collection("AppTest collection")]
    public class GetTenureByIdTests : IDisposable
    {
        private readonly IDynamoDbFixture _dbFixture;
        private readonly TenureFixture _tenureFixture;
        private readonly GetTenureStep _steps;

        public GetTenureByIdTests(MockWebApplicationFactory<Startup> appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _tenureFixture = new TenureFixture(_dbFixture.DynamoDbContext, appFactory.SnsFixture.SimpleNotificationService);
            _steps = new GetTenureStep(appFactory.Client);
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
        public void GetTenureByIdFoundReturnsResponse(bool nullTenuredAssetType)
        {
            this.Given(g => _tenureFixture.GivenATenureExist(nullTenuredAssetType))
                .When(w => _steps.WhenTenureDetailsAreRequested(_tenureFixture.TenureId.ToString()))
                .Then(t => _steps.ThenTheTenureDetailsAreReturned(_tenureFixture.Tenure))
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsNotFoundIfPersonNotExist()
        {
            this.Given(g => _tenureFixture.GivenATenureDoesNotExist())
                .When(w => _steps.WhenTenureDetailsAreRequested(_tenureFixture.TenureId.ToString()))
                .Then(t => _steps.ThenNotFoundIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsBadRequestIfIdInvalid()
        {
            this.Given(g => _tenureFixture.GivenAnInvalidTenureId())
                .When(w => _steps.WhenTenureDetailsAreRequested(_tenureFixture.InvalidTenureId))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }
    }
}
