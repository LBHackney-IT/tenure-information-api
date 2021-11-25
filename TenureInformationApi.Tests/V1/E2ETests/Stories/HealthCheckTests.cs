using System;
using TenureInformationApi.Tests.V1.E2ETests.Steps;
using TestStack.BDDfy;
using Xunit;

namespace TenureInformationApi.Tests.V1.E2ETests.Stories
{
    [Story(
        AsA = "Api client",
        IWant = "to be able to validate that the service satus is healty",
        SoThat = "I can be sure that calls made to it will succeed.")]
    [Collection("AppTest collection")]
    public class HealthCheckTests : IDisposable
    {
        private readonly HealthCheckSteps _steps;

        public HealthCheckTests(MockWebApplicationFactory<Startup> appFactory)
        {
            _steps = new HealthCheckSteps(appFactory.Client);
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
                if (null != _steps)
                    _steps.Dispose();
                _disposed = true;
            }
        }

        [Fact]
        public void ServiceReturnsHealthyStatus()
        {
            this.Given("A running service")
                .When(w => _steps.WhenTheHealtchCheckIsCalled())
                .Then(t => _steps.ThenTheHealthyStatusIsReturned())
                .BDDfy();
        }
    }
}
