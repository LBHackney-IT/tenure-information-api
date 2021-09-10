using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.Tests.V1.E2ETests.Fixtures;
using TenureInformationApi.Tests.V1.E2ETests.Steps;
using TenureInformationApi.V1.Boundary.Requests;
using TestStack.BDDfy;
using Xunit;

namespace TenureInformationApi.Tests.V1.E2ETests.Stories
{
    [Story(
        AsA = "Internal Hackney user (such as a Housing Officer or Area housing Manager)",
        IWant = "the ability to edit details of a tenure",
        SoThat = "I can ensure that the tenure details are up to date"
    )]
    [Collection("Aws collection")]
    public class EditTenureDetailsTests : IDisposable
    {
        private readonly AwsIntegrationTests<Startup> _dbFixture;

        private readonly TenureFixture _tenureFixture;
        private readonly EditTenureDetailsStep _steps;
        private readonly Fixture _fixture = new Fixture();

        public EditTenureDetailsTests(AwsIntegrationTests<Startup> dbFixture)
        {
            _dbFixture = dbFixture;
            _tenureFixture = new TenureFixture(_dbFixture.DynamoDbContext, _dbFixture.SimpleNotificationService);
            _steps = new EditTenureDetailsStep(_dbFixture.Client);
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
        public void ServiceReturns400BadRequest()
        {
            var invalidRequestObject = CreateInvalidRequestObject();

            this.Given(g => _tenureFixture.GivenATenureExist(false))
                .When(w => _steps.WhenEditTenureDetailsApiIsCalled(_tenureFixture.TenureId, invalidRequestObject))
                .Then(t => _steps.ThenBadRequestIsReturned())
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

        [Fact]
        public void ServiceReturns204AndUpdatesDatabase()
        {
            var requestObject = CreateValidRequestObject();

            this.Given(g => _tenureFixture.GivenATenureExist(false))
                .When(w => _steps.WhenEditTenureDetailsApiIsCalled(_tenureFixture.TenureId, requestObject))
                .Then(t => _steps.ThenNoContentResponseReturned())
                .And(a => _steps.TheTenureHasBeenUpdatedInTheDatabase(_tenureFixture, requestObject))
                .BDDfy();
        }

        private EditTenureDetailsRequestObject CreateValidRequestObject()
        {
            var tenureStartDate = _fixture.Create<DateTime>();
            var tenureEndDate = tenureStartDate.AddDays(7);

            return _fixture.Build<EditTenureDetailsRequestObject>()
                .With(x => x.StartOfTenureDate, tenureStartDate)
                .With(x => x.EndOfTenureDate, tenureEndDate)
                .Create();
        }

        private EditTenureDetailsRequestObject CreateInvalidRequestObject()
        {
            var tenureStartDate = _fixture.Create<DateTime>();
            var tenureEndDate = tenureStartDate.AddDays(-7);

            return _fixture.Build<EditTenureDetailsRequestObject>()
                .With(x => x.StartOfTenureDate, tenureStartDate)
                .With(x => x.EndOfTenureDate, tenureEndDate)
                .Create();
        }
    }
}
