using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace TenureInformationApi.Tests.V1.Factories
{
    public class ResponseFactoryTest
    {
        private Fixture _fixture = new Fixture();
        [Fact]
        public void CanMapADatabaseEntityToADomainObject()
        {
            var domain = _fixture.Create<TenureInformation>();
            var response = domain.ToResponse();
            domain.Should().BeEquivalentTo(response);

        }
    }
}
